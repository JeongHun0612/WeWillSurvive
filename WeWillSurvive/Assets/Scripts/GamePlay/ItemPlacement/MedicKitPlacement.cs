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

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private Sprite[] _medickitSprites;

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

                _name = "SpecialMedicalKit";
                Count = ItemManager.GetItemCount(EItem.SpecialMedicKit);
            }
            else
            {
                _itemObjects[0].GetComponent<Image>().sprite = _medickitSprites[(int)EMedicalKitSpriteType.Normal];
                _itemObjects[0].gameObject.SetActive(count != 0f);

                _name = "MedicalKit";
                Count = ItemManager.GetItemCount(EItem.MedicKit);
            }
        }
    }
}
