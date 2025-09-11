using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Ending;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class RationPanel : PagePanel
    {
        [Header("## Food")]
        [SerializeField] private Image[] _foodImages;
        [SerializeField] private TMP_Text _foodOverflowText;

        [Header("## Water")]
        [SerializeField] private Image[] _waterImages;
        [SerializeField] private TMP_Text _waterOverflowText;

        [Header("## Ration Characters")]
        [SerializeField] private List<RationCharacter> _rationCharacters;

        [Header("## Special Item")]
        [SerializeField] private RationItem _specialFoodItem;
        [SerializeField] private RationItem _specialMedicKit;

        private Dictionary<EItem, float> _dailyItemCounts = new();

        private EventBus EventBus => ServiceLocator.Get<EventBus>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public async override UniTask InitializeAsync()
        {
            PanelType = EPanelType.Ration;
            PageCount = 1;

            foreach (var rationCharacter in _rationCharacters)
            {
                await rationCharacter.InitializeAsync();
                rationCharacter.RationItemsRegisterEvent(this);
            }

            _specialFoodItem.Initialize();
            _specialFoodItem.RegisterEvent(OnClickSpecialFoodItem);

            _specialMedicKit.Initialize();
            _specialMedicKit.RegisterEvent(OnClickSpecialMedicKitItem);

            _dailyItemCounts = new()
            {
                [EItem.Food] = 0f,
                [EItem.Water] = 0f,
                [EItem.MedicKit] = 0f,
                [EItem.SpecialFood] = 0f,
                [EItem.SpecialMedicKit] = 0f,
            };

            // 이벤트 등록
            EventBus.Subscribe<EndDayEvent>(OnEndDayEvent);
            EventBus.Subscribe<ChoiceOptionSelectedEvent>(OnChoiceOptionSelectedEvent);
            //EventBus.Subscribe<RationItemSelectedEvent>(OnRationItemSelectedEvent);

            await UniTask.CompletedTask;
        }

        public override async UniTask RefreshPageAsync(int startPageIndex)
        {
            await base.RefreshPageAsync(startPageIndex);

            PageCount = (EndingManager.Instance.IsEnding) ? 0 : 1;

            foreach (var rationCharacter in _rationCharacters)
            {
                rationCharacter.Refresh();
            }

            // 금일 아이템 갯수 저장
            SetDailyItemCount();

            // RationItem Panel 업데이트
            UpdateSpecialItemPanel();
            UpdateFoodPanel();
            UpdateWaterPanel();
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);
        }

        public void OnClickFoodItem(RationItem rationItem)
        {
            if (_specialFoodItem.IsSelected)
                OnClickSpecialFoodItem();

            var foodItemCount = GetDailyItemCount(EItem.Food);
            HandleBasicItemToggle(rationItem, foodItemCount, UpdateFoodPanel);
        }

        public void OnClickWaterItem(RationItem rationItem)
        {
            if (_specialFoodItem.IsSelected)
                OnClickSpecialFoodItem();

            var waterItemCount = GetDailyItemCount(EItem.Water);
            HandleBasicItemToggle(rationItem, waterItemCount, UpdateWaterPanel);
        }

        public void OnClickMedicKitItem(RationItem rationItem)
        {
            if (_specialMedicKit.IsSelected)
                OnClickSpecialMedicKitItem();

            var medicKitItemCount = GetDailyItemCount(EItem.MedicKit);
            HandleBasicItemToggle(rationItem, medicKitItemCount);
        }

        public void OnClickSpecialFoodItem(RationItem rationItem = null)
        {
            if (rationItem == null)
                rationItem = _specialFoodItem;

            HandleSpecialItemToggle(
                rationItem,
                updateUI: () => { UpdateFoodPanel(); UpdateWaterPanel(); },
                rc => rc.FoodItem,
                rc => rc.WaterItem
            );
        }

        public void OnClickSpecialMedicKitItem(RationItem rationItem = null)
        {
            if (rationItem == null)
                rationItem = _specialMedicKit;

            HandleSpecialItemToggle(
                rationItem,
                updateUI: null,
                rc => rc.MedicKitItem
            );
        }

        private void SetDailyItemCount()
        {
            _dailyItemCounts = _dailyItemCounts.ToDictionary(pair => pair.Key, pair => ItemManager.GetItemCount(pair.Key));
        }

        private void HandleBasicItemToggle(RationItem rationItem, float itemCount, Action updateUI = null)
        {
            EItem item = rationItem.Item;
            float usageAmount = rationItem.UsageAmount;
            bool targetSelected = !rationItem.IsSelected;

            if (!rationItem.IsSelected && itemCount < usageAmount)
            {
                Debug.LogWarning($"[{rationItem.Item}] 아이템 수량이 부족합니다. 남은 갯수 : {itemCount}");
                return;
            }

            UpdateDailyItemCount(item, (targetSelected ? -usageAmount : +usageAmount));

            rationItem.OnSelected(targetSelected);

            EventBus.Publish(new RationItemSelectedEvent
            {
                Item = item,
                IsSelected = targetSelected,
                RemainCount = GetDailyItemCount(item),
            });

            updateUI?.Invoke();
        }

        private void HandleSpecialItemToggle(RationItem rationItem, Action updateUI, params Func<RationCharacter, RationItem>[] picks)
        {
            bool targetSelected = !rationItem.IsSelected;

            foreach (var rationCharacter in _rationCharacters)
            {
                foreach (var pick in picks)
                {
                    if (pick == null)
                        continue;

                    var item = pick(rationCharacter);
                    if (item == null) continue;

                    if (item.IsSelected)
                    {
                        UpdateDailyItemCount(item.Item, item.UsageAmount);
                        item.OnSelected(false);
                    }

                    item.UpdateSprite(targetSelected);
                }
            }

            UpdateDailyItemCount(rationItem.Item, rationItem.UsageAmount);
            rationItem.OnSelected(targetSelected);

            EventBus.Publish(new RationItemSelectedEvent
            {
                Item = rationItem.Item,
                IsSelected = targetSelected,
                RemainCount = GetDailyItemCount(rationItem.Item),
            });

            updateUI?.Invoke();
        }

        private void UpdateSpecialItemPanel()
        {
            // 우주 특별식 셋팅
            var hasSpecialFood = GetDailyItemCount(EItem.SpecialFood) != 0f;
            if (hasSpecialFood)
                _specialFoodItem.Refresh();

            _specialFoodItem.gameObject.SetActive(hasSpecialFood);

            // 만능 의료 키트 셋팅
            var hasSpecialMedicKit = GetDailyItemCount(EItem.SpecialMedicKit) != 0f;
            if (hasSpecialMedicKit)
                _specialMedicKit.Refresh();

            _specialMedicKit.gameObject.SetActive(hasSpecialMedicKit);
        }

        private void UpdateFoodPanel()
        {
            float foodItemCount = GetDailyItemCount(EItem.Food);
            UpdateRationItemPanel(_foodImages, _foodOverflowText, foodItemCount);
        }

        private void UpdateWaterPanel()
        {
            float waterItemCount = GetDailyItemCount(EItem.Water);
            UpdateRationItemPanel(_waterImages, _waterOverflowText, waterItemCount);
        }

        private void UpdateRationItemPanel(Image[] targetImages, TMP_Text overflowText, float itemCount)
        {
            int targetItemCount = targetImages.Length;

            for (int i = 0; i < targetItemCount; i++)
            {
                targetImages[i].fillAmount = Mathf.Clamp01(itemCount - i);
            }

            float overflow = itemCount - targetItemCount;

            if (overflow > 0f)
            {
                overflowText.gameObject.SetActive(true);
                overflowText.text = $"+ {overflow:0.##}";
            }
            else
            {
                overflowText.gameObject.SetActive(false);
            }
        }

        private void UpdateDailyItemCount(EItem item, float usageAmount)
        {
            if (_dailyItemCounts.ContainsKey(item))
            {
                var itemCount = _dailyItemCounts[item];
                itemCount += usageAmount;
                _dailyItemCounts[item] = itemCount;
            }
        }

        private float GetDailyItemCount(EItem item)
        {
            if (_dailyItemCounts.TryGetValue(item, out float itemCount))
            {
                return itemCount;
            }

            return 0f;
        }

        private void OnEndDayEvent(EndDayEvent context)
        {
            bool useSpecialFood = _specialFoodItem.IsSelected;
            bool useSpecialMedicKit = _specialMedicKit.IsSelected;

            // 우주 특별식을 사용한 경우
            if (useSpecialFood)
                _specialFoodItem.UsedItem();

            // 만능의료키트를 사용한 경우
            if (useSpecialMedicKit)
                _specialMedicKit.UsedItem();

            foreach (var rationCharacter in _rationCharacters)
            {
                if (!useSpecialFood)
                {
                    rationCharacter.ApplyFoodItem();
                    rationCharacter.ApplyWaterItem();
                }

                if (!useSpecialMedicKit)
                {
                    rationCharacter.ApplyMedicKitItem();
                }
            }
        }

        private void OnChoiceOptionSelectedEvent(ChoiceOptionSelectedEvent context)
        {
            var requiredAmount = (context.IsSelected ? -context.RequiredAmount : +context.RequiredAmount);
            UpdateDailyItemCount(context.Item, requiredAmount);

            if (context.Item == EItem.Food)
                UpdateFoodPanel();
            else if (context.Item == EItem.Water)
                UpdateWaterPanel();
        }
    }
}
