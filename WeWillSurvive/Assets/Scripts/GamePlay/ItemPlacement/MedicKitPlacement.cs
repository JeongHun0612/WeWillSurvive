using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Item;

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

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

        public async override UniTask InitializeAsync()
        {
            await base.InitializeAsync();

            _medickitSprites = new Sprite[2];
            _medickitSprites[0] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/medical_kit.png");
            _medickitSprites[1] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/special_medical_kit.png");
        }

        public override void Initialize()
        {
            base.Initialize();

            _medickitSprites = new Sprite[2];
            _medickitSprites[0] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "medical_kit");
            _medickitSprites[1] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "special_medical_kit");
        }

        public override void UpdateItemPlacement(float count)
        {
            ItemObjectAllDeactivate();

            if (_itemObjects == null || _itemObjects.Count == 0)
                return;

            bool hasSuperMedicKit = ItemManager.HasItem(EItem.SpecialMedicKit);

            if (hasSuperMedicKit)
            {
                _itemObjects[0].GetComponent<Image>().sprite = _medickitSprites[(int)EMedicalKitSpriteType.Special];
                _itemObjects[0].gameObject.SetActive(true);

                _itemType = EItem.SpecialMedicKit;
                Count = ItemManager.GetItemCount(EItem.SpecialMedicKit);
            }
            else
            {
                _itemObjects[0].GetComponent<Image>().sprite = _medickitSprites[(int)EMedicalKitSpriteType.Normal];
                _itemObjects[0].gameObject.SetActive(count != 0f);

                _itemType = EItem.MedicKit;
                Count = ItemManager.GetItemCount(EItem.MedicKit);
            }
        }
    }
}
