using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public override void Initialize()
        {
            PanelType = EPanelType.Ration;
            PageCount = 1;

            foreach (var rationCharacter in _rationCharacters)
            {
                rationCharacter.Initialize();
                rationCharacter.RegisterEvent(this);
            }
        }

        public override async UniTask RefreshPageAsync(int startPageIndex)
        {
            await base.RefreshPageAsync(startPageIndex);

            foreach (var rationCharacter in _rationCharacters)
            {
                rationCharacter.Refresh();
            }
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);

            UpdateFoodItemCount();
            UpdateWaterItemCount();
        }

        public override void ApplyResult()
        {
            foreach (var rationCharacter in _rationCharacters)
            {
                rationCharacter.ApplyRationItem();
            }
        }

        public void UpdateFoodItemCount()
        {
            float foodItemCount = ItemManager.GetItemCount(EItem.Food);
            UpdateItemCount(_foodImages, _foodOverflowText, foodItemCount);
        }

        public void UpdateWaterItemCount()
        {
            float waterItemCount = ItemManager.GetItemCount(EItem.Water);
            UpdateItemCount(_waterImages, _waterOverflowText, waterItemCount);
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
                overflowText.text = $"+ {overflow:0.#}";
            }
            else
            {
                overflowText.gameObject.SetActive(false);
            }
        }
    }
}
