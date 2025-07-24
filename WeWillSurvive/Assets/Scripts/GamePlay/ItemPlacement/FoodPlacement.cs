using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class FoodPlacement : ItemPlacement
    {
        public enum EFoodSpriteType
        {
            Normal, Used
        }

        private const int FOODBOX_ACTIVATE_COUNT = 7;
        private const int SPECIALFOOD_INDEX = 7;

        private Sprite[] _foodSprites;
        private Sprite[] _foodBoxSprites;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public override void Initialize()
        {
            base.Initialize();

            _foodSprites = new Sprite[2];
            _foodSprites[0] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "food1");
            _foodSprites[1] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "food2");

            _foodBoxSprites = new Sprite[5];
            _foodBoxSprites[0] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "foodbox_1");
            _foodBoxSprites[1] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "foodbox_2");
            _foodBoxSprites[2] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "foodbox_3");
            _foodBoxSprites[3] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "foodbox_4");
            _foodBoxSprites[4] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "foodbox_5");
        }

        public override void UpdateItemPlacement(float count)
        {
            Count = count;
            ItemObjectAllDeactivate();

            if (_itemObjects == null || _itemObjects.Count == 0)
                return;

            int itemCeilCount = Mathf.CeilToInt(count);
            int itemCount = Mathf.Min(FOODBOX_ACTIVATE_COUNT, itemCeilCount);

            for (int i = 0; i < itemCount; i++)
            {
                _itemObjects[i].SetActive(true);

                if (i >= FOODBOX_ACTIVATE_COUNT - 1)
                {
                    int foodBoxIndex = Mathf.Min(itemCeilCount - FOODBOX_ACTIVATE_COUNT, _foodBoxSprites.Length - 1);
                    _itemObjects[i].GetComponent<Image>().sprite = _foodBoxSprites[foodBoxIndex];
                }
            }

            // 첫번째 Food의 Sprite 적용 (Normal, Used)
            float decimalCount = count - Mathf.FloorToInt(count);
            EFoodSpriteType foodSpriteType = GetFoodSpriteType(decimalCount);
            _itemObjects[0].GetComponent<Image>().sprite = _foodSprites[(int)foodSpriteType];

            // SpecialFood를 소지하고 있으면 활성화
            if (ItemManager.HasItem(EItem.SpecialFood))
            {
                _itemObjects[SPECIALFOOD_INDEX].SetActive(true);
            }
        }

        private EFoodSpriteType GetFoodSpriteType(float value)
        {
            if (value == 0f || value == 1f)
                return EFoodSpriteType.Normal;
            else
                return EFoodSpriteType.Used;
        }
    }
}
