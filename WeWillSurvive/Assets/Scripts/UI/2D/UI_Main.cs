using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using WeWillSurvive.Item;
using System.Collections.Generic;

namespace WeWillSurvive
{
    public class UI_Main : UI_Popup
    {
        [SerializeField] private Button _roomMonitorButton;
        [SerializeField] private Button _nextDayButton;
        [SerializeField] private Button _projecterButton;
        [SerializeField] private TextMeshProUGUI _dayText;

        [Header("## Characters")]
        [SerializeField] private List<UI_Character> _charcaterUIs;

        [Header("Item Placements")]
        [SerializeField] private List<ItemPlacement> _itemPlacements;

        private ItemManager _itemManager;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public override async UniTask InitializeAsync()
        {
            foreach (var characterUI in _charcaterUIs)
            {
                characterUI.Initialize();
            }

            foreach (var itemPlacement in _itemPlacements)
            {
                itemPlacement.Initialize();
            }

            // Room Monitor
            _roomMonitorButton.onClick.AddListener(() => UIManager.Instance.ShowPopup<UI_RoomMonitor>());

            // Projecter
            _projecterButton.onClick.AddListener(() =>
            {
                UIManager.Instance.ClosePopups(remain: 1);
                UIManager.Instance.ShowPopup<UI_Projecter>();
            });

            // Next Day
            _nextDayButton.onClick.AddListener(() => GameManager.Instance.StartNextDay());

            EventBus.Subscribe<NewDayEvent>(OnNewDayEvent);

            _itemManager = ServiceLocator.Get<ItemManager>();

            await UniTask.Yield();
        }

        public override void OnShow()
        {
            UpdateUI();
        }

        public void UseItemDebug(int type)
        {
            //_itemManager.UsedItem(ServiceLocator.Get<CharacterManager>().Characters[ECharacter.Bell], (EItem)type, 0.25f);
            var target = ServiceLocator.Get<CharacterManager>().Characters[ECharacter.Lead];
            _itemManager.UsedItem(target, (EItem)type, 1f);
            UpdateUI();
        }

        public void GetItemDebug(int type)
        {
            _itemManager.AddItem((EItem)type);
            UpdateUI();
        }

        private void UpdateUI()
        {
            _dayText.text = "Day " + GameManager.Instance.Day;

            // 캐릭터 업데이트
            foreach (var characterUI in _charcaterUIs)
            {
                characterUI.UpdateCharacterImage();
            }

            // 아이템 업데이트
            foreach (var itemPlacement in _itemPlacements)
            {
                float itemCount = ItemManager.GetItemCount(itemPlacement.ItemType);
                itemPlacement.UpdateItemPlacement(itemCount);
            }
        }

        private void OnNewDayEvent(NewDayEvent context)
        {
            UpdateUI();
        }
    }
}