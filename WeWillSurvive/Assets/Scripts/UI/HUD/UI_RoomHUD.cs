using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Ending;
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
        [SerializeField] private GameObject _buffListPanel;
        [SerializeField] private List<TMP_Text> _buffTextList;

        [Header("Ending UI")]
        [SerializeField] private GameObject _titleButton;

        private ERoom _currentRoom;

        private Dictionary<ECharacter, Sprite> _characterIcons = new();

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();
        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            EventBus.Subscribe<NewDayEvent>(OnNewDayEvent);
            EventBus.Subscribe<MoveRoomCompleteEvent>(OnMoveRoomCompleteEvent);

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

        private void UpdateBuffPanel()
        {
            _buffListPanel.SetActive(_currentRoom == ERoom.Main);
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

            _buffListPanel.SetActive(true);
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
            SoundManager.Instance.PlaySFX(ESFX.SFX_Click_2);

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

        public void OnClickSetting()
        {
            SoundManager.Instance.PlaySFX(ESFX.SFX_Click_2);

            UIManager.Instance.CloseAllPopups();
            UIManager.Instance.ShowPopup<UI_InGameSetting>();
        }

        public void OnClickTitle()
        {
            SoundManager.Instance.PlaySFX(ESFX.SFX_Click_2);

            GameManager.Instance.OnMoveTitle();
        }

        private void OnNewDayEvent(NewDayEvent context)
        {
            _currentRoom = ERoom.Main;

            var isEnding = EndingManager.Instance.IsEnding;
            _titleButton.SetActive(isEnding);

            if (isEnding)
            {
                _dayText.text = (EndingManager.Instance.EndingType == EEndingType.DeathByStarvation) ? "생존 실패" : "생존 성공";
                _roomMoveButton.SetActive(false);
                _buffListPanel.SetActive(false);
                return;
            }

            _dayText.text = $"Day {context.CurrentDay}";
            SetRoomMoveButton();
            UpdateBuffListText();
        }

        private void OnMoveRoomCompleteEvent(MoveRoomCompleteEvent context)
        {
            _currentRoom = context.CurrentRoom;
            UpdateRoomMoveButton();
            UpdateBuffPanel();
        }
    }
}