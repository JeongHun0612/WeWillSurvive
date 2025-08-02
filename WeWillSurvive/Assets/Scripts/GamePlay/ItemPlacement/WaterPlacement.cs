using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class WaterPlacement : ItemPlacement
    {
        public enum EWaterSpriteType
        {
            Full, Half, Little
        }

        private Sprite[] _waterSprites;
        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

        public async override UniTask InitializeAsync()
        {
            await base.InitializeAsync();

            _waterSprites = new Sprite[3];
            _waterSprites[0] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/water1.png");
            _waterSprites[1] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/water2.png");
            _waterSprites[2] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Items/Item_Normal/water3.png");
        }

        public override void Initialize()
        {
            base.Initialize();

            _waterSprites = new Sprite[3];
            _waterSprites[0] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "water1");
            _waterSprites[1] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "water2");
            _waterSprites[2] = SpriteManager.Instance.GetSprite(ESpriteAtlas.Item_Atlas, "water3");
        }

        public override void UpdateItemPlacement(float count)
        {
            Count = count;
            ItemObjectAllDeactivate();

            if (_itemObjects == null || _itemObjects.Count == 0)
                return;

            int itemCount = Mathf.Min(_itemObjects.Count, Mathf.CeilToInt(count));

            for (int i = 0; i < itemCount; i++)
            {
                _itemObjects[i].SetActive(true);
            }

            float decimalCount = count - Mathf.FloorToInt(count);
            EWaterSpriteType waterSpriteType = GetWaterSpriteType(decimalCount);
            if ((int)waterSpriteType < _waterSprites.Length)
            {
                _itemObjects[0].GetComponent<Image>().sprite = _waterSprites[(int)waterSpriteType];
            }
        }

        private EWaterSpriteType GetWaterSpriteType(float value)
        {
            if (value == 0f || value == 1f)
                return EWaterSpriteType.Full;

            if (value <= 0.25f) return EWaterSpriteType.Little;
            else if (value <= 0.75f) return EWaterSpriteType.Half;
            else return EWaterSpriteType.Full;
        }
    }
}
