using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using WeWillSurvive.Character;
using static Define;

namespace WeWillSurvive
{
    public struct ClickEvent { }

    public class UI_Background : UI_Scene
    {
        [Header("UI")]
        [SerializeField] Image _backgroundImage;
        [SerializeField] Image _lightOffImage;
        [SerializeField] Button _leftButton;
        [SerializeField] Button _rightButton;
        [SerializeField] Sprite[] _backgroundSprites;

        int _currentRoomIdx;
        bool _changingBackground;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            // 배경 클릭하면 Popup UI 닫음 (UI_Main/UI_Room은 남겨놓음)
            _backgroundImage.GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.ClosePopups(remain: 1));
            _leftButton.onClick.AddListener(() => ChangeBackground((ERoom)(_currentRoomIdx - 1)));
            _rightButton.onClick.AddListener(() => ChangeBackground((ERoom)(_currentRoomIdx + 1)));

            EventBus.Subscribe<MoveRoomEvent>(OnMoveRoomEvent);

            await UniTask.CompletedTask;
        }

        public override void OnShow()
        {
            _changingBackground = false;

            SetBackground(ERoom.Main);
        }

        public void ChangeBackground(ERoom roomName)
        {
            // 변경 중이면 취소
            if (_changingBackground)
                return;

            _changingBackground = true;

            // Wipe
            UIManager.Instance.ShowOverlay<UI_Black>().Wipe(right: (int)roomName < _currentRoomIdx, 
                coverAction: () => SetBackground(roomName), finishAction: () => _changingBackground = false);
        }

        public void LightOff()
        {
            _lightOffImage.enabled = true;
        }

        private void SetBackground(ERoom roomName)
        {
            // 배경 이미지 변경
            _currentRoomIdx = (int)roomName;
            _backgroundImage.sprite = _backgroundSprites[_currentRoomIdx];

            // 버튼 조건 체크
            _leftButton.interactable = _currentRoomIdx > 0;
            _rightButton.interactable = _currentRoomIdx < (int)ERoom.MaxCount - 1;

            // Popup UI 초기화
            UIManager.Instance.CloseAllPopups();

            if (roomName == ERoom.Main)     // 메인 로비인 경우
            {
                // 방 불 항상 켜짐
                _lightOffImage.enabled = false;

                // Popup UI
                UIManager.Instance.ShowPopup<UI_Main>();
            }
            else                            // 메인 로비 아닌 경우
            {
                ECharacter player;
                switch (roomName)
                {
                    case ERoom.Lead:
                        player = ECharacter.Lead; break;
                    case ERoom.Cook:
                        player = ECharacter.Cook; break;
                    case ERoom.DrK:
                        player = ECharacter.DrK; break;
                    case ERoom.Bell:
                        player = ECharacter.Bell; break;
                    default:
                        return;
                }

                // 우주 기지 내 존재하지 않거나 죽으면 방 불 꺼짐
                CharacterState state = ServiceLocator.Get<CharacterManager>().GetCharacter(player)?.State;
                _lightOffImage.enabled = state.HasState(EState.Exploring) || state.HasState(EState.Dead);

                // Popup UI
                string name = Enum.GetName(typeof(ECharacter), player);
                UIManager.Instance.ShowPopup<UI_Room>().SetupRoomUI(player);
            }
        }
        private void OnMoveRoomEvent(MoveRoomEvent context)
        {
            ChangeBackground(context.TargetRoom);
        }
    }
}
