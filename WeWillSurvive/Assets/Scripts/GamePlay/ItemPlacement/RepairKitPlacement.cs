using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.Util;

namespace WeWillSurvive
{
    public class RepairKitPlacement : ItemPlacement
    {
        public enum ERepairKitSpriteType
        {
            Normal,
            Special,
        }

        private Sprite[] _repairkitSprites;

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

        public async override UniTask InitializeAsync()
        {
            await base.InitializeAsync();

            _repairkitSprites = new Sprite[2];
            _repairkitSprites[0] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/repair_kit.png");
            _repairkitSprites[1] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/special_repair_kit.png");
        }

        public override void UpdateItemPlacement()
        {
            if (_itemObjects == null || _itemObjects.Count == 0)
                return;

            bool hasSuperRepairKit = ItemManager.HasItem(EItem.SpecialRepairKit);
            ItemType = hasSuperRepairKit ? EItem.SpecialRepairKit : EItem.RepairKit;
            Count = ItemManager.GetItemCount(ItemType);
            ItemObjectAllDeactivate();

            ItemObjectActivate(Count);
        }

        protected override void ItemObjectActivate(float count)
        {
            Sprite changeSprite = null;
            if (ItemType == EItem.RepairKit)
            {
                changeSprite = _repairkitSprites[(int)ERepairKitSpriteType.Normal];
            }
            else if (ItemType == EItem.SpecialRepairKit)
            {
                changeSprite = _repairkitSprites[(int)ERepairKitSpriteType.Special];
            }

            _itemObjects[0].GetComponent<Image>().sprite = changeSprite;
            _itemObjects[0].SetActive(count != 0f);
        }

        protected override string BuildStatusText()
        {
            bool hasSuperRepairKit = ItemManager.HasItem(EItem.SpecialRepairKit);
            bool hasRepairKit = ItemManager.HasItem(EItem.RepairKit);

            if (hasSuperRepairKit && hasRepairKit)
                return $"{EnumUtil.GetInspectorName(EItem.RepairKit)} &\n{EnumUtil.GetInspectorName(EItem.SpecialRepairKit)}";

            return $"{EnumUtil.GetInspectorName(_itemType)}";
        }

        //public override void UpdateItemPlacement()
        //{
        //    ItemObjectAllDeactivate();

        //    if (_itemObjects == null || _itemObjects.Count == 0)
        //        return;

        //    bool hasSuperRepairKit = ItemManager.HasItem(EItem.SpecialRepairKit);

        //    if (hasSuperRepairKit)
        //    {
        //        _itemObjects[0].GetComponent<Image>().sprite = _repairkitSprites[(int)ERepairKitSpriteType.Special];
        //        _itemObjects[0].gameObject.SetActive(true);

        //        _itemType = EItem.SpecialRepairKit;
        //        Count = ItemManager.GetItemCount(EItem.SpecialRepairKit);
        //    }
        //    else
        //    {
        //        _itemObjects[0].GetComponent<Image>().sprite = _repairkitSprites[(int)ERepairKitSpriteType.Normal];
        //        _itemObjects[0].gameObject.SetActive(Count != 0f);

        //        _itemType = EItem.RepairKit;
        //        Count = ItemManager.GetItemCount(EItem.RepairKit);
        //    }
        //}
    }
}
