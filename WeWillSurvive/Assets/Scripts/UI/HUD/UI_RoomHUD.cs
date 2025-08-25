using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.GameEvent;
using WeWillSurvive.UI;
using WeWillSurvive.Util;

namespace WeWillSurvive
{
    public class UI_RoomHUD : UI_HUD
    {
        [Header("Day Text")]
        [SerializeField] private TMP_Text _dayText;

        [Header("RoomMoveButton")]
        [SerializeField] private GameObject _roomMoveButton;
        [SerializeField] private TMP_Text _roomMoveButtonText;
        [SerializeField] private Image _roomMoveButtonCharacterIcon;

        [Header("Buff List")]
        [SerializeField] private List<TMP_Text> _buffTextList;

        [Header("MoveButton")]
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        private ERoom _currentRoom;

        private Dictionary<ECharacter, Sprite> _characterIcons = new();

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();
        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            EventBus.Subscribe<NewDayEvent>(OnNewDayEvent);
            EventBus.Subscribe<MoveRoomCompleteEvent>(OnMoveRoomCompleteEvent);

            _leftButton.onClick.AddListener(() => MoveRoom(_currentRoom - 1));
            _rightButton.onClick.AddListener(() => MoveRoom(_currentRoom + 1));

            _characterIcons = new()
            {
                { ECharacter.Lead, await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/Lead_Icon_Normal.png") },
                { ECharacter.Cook, await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/Cook_Icon_Normal.png") },
                { ECharacter.Bell, await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/Bell_Icon_Normal.png") },
                { ECharacter.DrK, await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/DrK_Icon_Normal.png") },
            };

            await UniTask.Yield();
        }

        private void MoveRoom(ERoom targetRoom)
        {
            // targetRoom이 범위를 벗어나면
            if (targetRoom < 0 || targetRoom >= ERoom.MaxCount)
                return;

            EventBus.Publish(new MoveRoomEvent { TargetRoom = targetRoom });
        }

        private void UpdateMoveButton()
        {
            // 버튼 조건 체크
            _leftButton.interactable = _currentRoom > 0;
            _rightButton.interactable = _currentRoom < ERoom.MaxCount - 1;
        }

        private void SetRoomMoveButton()
        {
            var dailyCharcterEvent = GameEventManager.Instance.DailyCharacterEvent;
            if (dailyCharcterEvent == null)
            {
                _roomMoveButton.SetActive(false);
            }
            else
            {
                _roomMoveButton.SetActive(true);

                // 발생한 캐릭터 이벤트에 맞는 캐릭터 아이콘 할당
                _roomMoveButtonCharacterIcon.sprite = GetCharacterIcon(dailyCharcterEvent.Character);

                UpdateRoomMoveButton();
            }
        }

        private void UpdateRoomMoveButton()
        {
            if (_currentRoom == ERoom.Main)
            {
                _roomMoveButtonText.text = "이벤트 수행";
                _roomMoveButtonCharacterIcon.gameObject.SetActive(true);
            }
            else
            {
                _roomMoveButtonText.text = "돌아가기";
                _roomMoveButtonCharacterIcon.gameObject.SetActive(false);
            }
        }

        private void UpdateBuffListText()
        {
            var activeBuffs = BuffManager.Instance.GetActiveBuffs();

            for (int i = 0; i < _buffTextList.Count; i++)
            {
                if (i < activeBuffs.Count)
                {
                    var buff = activeBuffs[i];
                    _buffTextList[i].text = $"{EnumUtil.GetInspectorName(buff.Effect)} [남은 일수 : {buff.Duration}]";
                    _buffTextList[i].gameObject.SetActive(true);
                }
                else
                {
                    _buffTextList[i].gameObject.SetActive(false);
                }
            }
        }

        private Sprite GetCharacterIcon(ECharacter characterType)
        {
            if (!_characterIcons.TryGetValue(characterType, out var icon))
            {
                Debug.LogWarning($"{characterType} 타입에 맞는 캐릭터 아이콘을 찾을 수 없습니다.");
                return null;
            }

            return icon;
        }

        public void OnClickRoomMove()
        {
            if (_currentRoom == ERoom.Main)
            {
                var characterEventCategory = GameEventManager.Instance.DailyCharacterEvent.Character;
                var targetRoom = EnumUtil.ParseEnum<ERoom>(characterEventCategory.ToString());

                EventBus.Publish(new MoveRoomEvent() { TargetRoom = targetRoom });
            }
            else
            {
                EventBus.Publish(new MoveRoomEvent() { TargetRoom = ERoom.Main });
            }
        }

        private void OnNewDayEvent(NewDayEvent context)
        {
            _currentRoom = ERoom.Main;
            _dayText.text = $"Day {context.CurrentDay}";

            UpdateMoveButton();
            SetRoomMoveButton();
            UpdateBuffListText();
        }

        private void OnMoveRoomCompleteEvent(MoveRoomCompleteEvent context)
        {
            _currentRoom = context.CurrentRoom;
            UpdateMoveButton();
            UpdateRoomMoveButton();
        }
    }
}