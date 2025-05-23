using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
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

        protected override void Init()
        {
            base.Init();
            SetBackground(ERoom.Main);

            // 배경 클릭하면 Popup UI 닫음 (UI_Main/UI_Room은 남겨놓음)
            _backgroundImage.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.ClosePopupUIs(1));
            _leftButton.onClick.AddListener(() => ChangeBackground((ERoom)(_currentRoomIdx - 1)));
            _rightButton.onClick.AddListener(() => ChangeBackground((ERoom)(_currentRoomIdx + 1)));
        }

        public void ChangeBackground(ERoom roomName)
        {
            // 변경 중이면 취소
            if (_changingBackground)
                return;

            _changingBackground = true;

            // Wipe
            GameManager.Instance.BlackUI.Wipe(right: (int)roomName < _currentRoomIdx, 
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
            GameManager.Instance.CloseAllPopupUI();

            if (roomName == ERoom.Main)     // 메인 로비인 경우
            {
                // 방 불 항상 켜짐
                _lightOffImage.enabled = false;

                // Popup UI
                ServiceLocator.Get<ResourceService>().LoadAsset("UI_Main").ContinueWith(prefab => Instantiate(prefab)).Forget();
            }
            else                            // 메인 로비 아닌 경우
            {
                ECharacter player = ECharacter.MaxCount;
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
                }

                // 우주 기지 내 존재하지 않거나 죽으면 방 불 꺼짐
                List<ECharacterState> state = CharacterManager.Instance.CharacterInfos[(int)player].State;
                _lightOffImage.enabled = state[0] == ECharacterState.None || state[0] == ECharacterState.Dead;

                // Popup UI
                string name = Enum.GetName(typeof(ECharacter), player);
                ServiceLocator.Get<ResourceService>().LoadAsset($"UI_{name}Room").ContinueWith(prefab =>
                {
                    GameObject go = Instantiate(prefab);
                    go.GetComponent<UI_Room>().SetupRoomUI(player);
                }).Forget();
            }
        }
    }
}
