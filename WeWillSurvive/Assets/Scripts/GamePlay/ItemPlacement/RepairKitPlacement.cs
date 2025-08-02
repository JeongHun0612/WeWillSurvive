using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Item;

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

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

        public async override UniTask InitializeAsync()
        {
            await base.InitializeAsync();

            _repairkitSprites = new Sprite[2];
            _repairkitSprites[0] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/repair_kit.png");
            _repairkitSprites[1] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/special_repair_kit.png");
        }

        public override void Initialize()
        {
            base.Initialize();

            _repairkitSprites = new Sprite[2];
            _repairkitSprites[0] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "repair_kit");
            _repairkitSprites[1] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "special_repair_kit");
        }

        public override void UpdateItemPlacement(float count)
        {
            ItemObjectAllDeactivate();

            if (_itemObjects == null || _itemObjects.Count == 0)
                return;

            bool hasSuperRepairKit = ItemManager.HasItem(EItem.SpecialRepairKit);

            if (hasSuperRepairKit)
            {
                _itemObjects[0].GetComponent<Image>().sprite = _repairkitSprites[(int)ERepairKitSpriteType.Special];
                _itemObjects[0].gameObject.SetActive(true);

                _name = "SpecialRepairKit";
                Count = ItemManager.GetItemCount(EItem.SpecialRepairKit);
            }
            else
            {
                _itemObjects[0].GetComponent<Image>().sprite = _repairkitSprites[(int)ERepairKitSpriteType.Normal];
                _itemObjects[0].gameObject.SetActive(count != 0f);
             
                _name = "RepairKit";
                Count = ItemManager.GetItemCount(EItem.RepairKit);
            }
        }
    }
}
