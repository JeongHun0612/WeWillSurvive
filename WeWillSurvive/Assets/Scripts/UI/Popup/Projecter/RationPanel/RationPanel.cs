using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
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
        [SerializeField] private RationItem _speicalFoodItem;
        [SerializeField] private RationItem _specialMedicKit;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public async override UniTask InitializeAsync()
        {
            PanelType = EPanelType.Ration;
            PageCount = 1;

            foreach (var rationCharacter in _rationCharacters)
            {
                await rationCharacter.InitializeAsync();
                rationCharacter.RegisterEvent(this);
            }

            _speicalFoodItem.Initialize();
            _speicalFoodItem.ItemSelectedEvent += OnClickSpecialFoodItem;

            _specialMedicKit.Initialize();
            _specialMedicKit.ItemSelectedEvent += OnClickSpecialMedicKitItem;

            // 이벤트 등록
            EventBus.Subscribe<EndDayEvent>(OnEndDayEvent);

            await UniTask.CompletedTask;
        }

        public override async UniTask RefreshPageAsync(int startPageIndex)
        {
            await base.RefreshPageAsync(startPageIndex);

            foreach (var rationCharacter in _rationCharacters)
            {
                rationCharacter.Refresh();
            }

            SpecialItemRefresh();

            // ItemCount 업데이트
            UpdateFoodItemCount();
            UpdateWaterItemCount();
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);
        }

        private void SpecialItemRefresh()
        {
            // 우주 특별식 셋팅
            var hasSpecialFood = ItemManager.HasItem(EItem.SpecialFood);
            if (hasSpecialFood)
                _speicalFoodItem.Refresh();

            _speicalFoodItem.gameObject.SetActive(hasSpecialFood);


            // 만능 의료 키트 셋팅
            var hasSpecialMedicKit = ItemManager.HasItem(EItem.SpecialMedicKit);
            if (hasSpecialMedicKit)
                _specialMedicKit.Refresh();

            _specialMedicKit.gameObject.SetActive(hasSpecialMedicKit);
        }

        private void UpdateFoodItemCount()
        {
            float itemCount = ItemManager.GetItemCount(EItem.Food);
            UpdateItemCount(_foodImages, _foodOverflowText, itemCount);
        }

        private void UpdateWaterItemCount()
        {
            float itemCount = ItemManager.GetItemCount(EItem.Water);
            UpdateItemCount(_waterImages, _waterOverflowText, itemCount);
        }

        private void UpdateItemCount(Image[] targetImages, TMP_Text overflowText, float itemCount)
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

        private void UpdateRationItemPanel(RationItem rationItem)
        {
            if (_speicalFoodItem.IsSelected)
            {
                _speicalFoodItem.OnSelected(false);

                foreach (var rationCharacter in _rationCharacters)
                {
                    rationCharacter.FoodItem.OnSelected(false, false);
                    rationCharacter.WaterItem.OnSelected(false, false);
                }
            }

            rationItem?.OnSelected(!rationItem.IsSelected);

            UpdateFoodItemCount();
            UpdateWaterItemCount();
        }

        public void OnClickFoodItem(RationItem rationItem) => UpdateRationItemPanel(rationItem);
        public void OnClickWaterItem(RationItem rationItem) => UpdateRationItemPanel(rationItem);
        public void OnClickMedicKitItem(RationItem rationItem)
        {
            if (_specialMedicKit.IsSelected)
            {
                _specialMedicKit.OnSelected(false);

                foreach (var rationCharacter in _rationCharacters)
                {
                    rationCharacter.MedicKitItem.OnSelected(false, false);
                }
            }

            rationItem?.OnSelected(!rationItem.IsSelected);
        }

        public void OnClickSpecialFoodItem(RationItem rationItem)
        {
            var targetSelected = !rationItem.IsSelected;

            foreach (var rationCharacter in _rationCharacters)
            {
                if (targetSelected)
                {
                    rationCharacter.FoodItem.OnSelected(false);
                    rationCharacter.WaterItem.OnSelected(false);
                }

                rationCharacter.FoodItem.OnSelected(targetSelected, false);
                rationCharacter.WaterItem.OnSelected(targetSelected, false);
            }

            if (rationItem != null)
                rationItem.OnSelected(targetSelected);

            var foodCount = ItemManager.GetItemCount(EItem.Food);
            var waterCount = ItemManager.GetItemCount(EItem.Water);

            UpdateItemCount(_foodImages, _foodOverflowText, foodCount);
            UpdateItemCount(_waterImages, _waterOverflowText, waterCount);
        }

        public void OnClickSpecialMedicKitItem(RationItem rationItem)
        {
            var targetSelected = !rationItem.IsSelected;

            foreach (var rationCharacter in _rationCharacters)
            {
                if (targetSelected)
                    rationCharacter.MedicKitItem.OnSelected(false);

                rationCharacter.MedicKitItem.OnSelected(targetSelected, false);
            }

            if (rationItem != null)
                rationItem.OnSelected(targetSelected);
        }

        private void OnEndDayEvent(EndDayEvent context)
        {
            bool useSpecialFood = _speicalFoodItem.IsSelected;
            bool useSpecialMedicKit = _specialMedicKit.IsSelected;


            // 우주 특별식을 사용한 경우
            if (useSpecialFood)
                _speicalFoodItem.UsedItem();

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
    }
}
