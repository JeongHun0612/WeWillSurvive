using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.Util;

namespace WeWillSurvive
{
    public class MedicKitPlacement : ItemPlacement
    {
        public enum EMedicalKitSpriteType
        {
            Normal,
            Special,
        }

        private Sprite[] _medickitSprites;

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

        public async override UniTask InitializeAsync()
        {
            await base.InitializeAsync();

            _medickitSprites = new Sprite[2];
            _medickitSprites[0] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/medical_kit.png");
            _medickitSprites[1] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/special_medical_kit.png");
        }

        public override void UpdateItemPlacement()
        {
            if (_itemObjects == null || _itemObjects.Count == 0)
                return;

            bool hasSuperMedicKit = ItemManager.HasItem(EItem.SpecialMedicKit);
            ItemType = hasSuperMedicKit ? EItem.SpecialMedicKit : EItem.MedicKit;
            Count = ItemManager.GetItemCount(ItemType);
            ItemObjectAllDeactivate();

            ItemObjectActivate(Count);
        }

        protected override void ItemObjectActivate(float count)
        {
            Sprite changeSprite = null;
            if (ItemType == EItem.MedicKit)
            {
                changeSprite = _medickitSprites[(int)EMedicalKitSpriteType.Normal];
            }
            else if (ItemType == EItem.SpecialMedicKit)
            {
                changeSprite = _medickitSprites[(int)EMedicalKitSpriteType.Special];
            }

            _itemObjects[0].GetComponent<Image>().sprite = changeSprite;
            _itemObjects[0].SetActive(count != 0f);
        }

        protected override string BuildStatusText()
        {
            bool hasSuperMedicKit = ItemManager.HasItem(EItem.SpecialMedicKit);
            bool hasMedicKit = ItemManager.HasItem(EItem.MedicKit);

            if (hasSuperMedicKit && hasMedicKit)
                return $"{EnumUtil.GetInspectorName(EItem.MedicKit)} &\n{EnumUtil.GetInspectorName(EItem.SpecialMedicKit)}";

            return $"{EnumUtil.GetInspectorName(_itemType)}";
        }
    }
}
