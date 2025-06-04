using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.ItemEffect;

namespace WeWillSurvive.Item
{
    public enum EItem
    {
        Food,
        SpecialFood,
        Water,
        MedicKit,
        SuperMedicKit,
        RepairKit,
        SuperRepairKit,
        NiceSpacesuit,
        Radio,
        LaserGun,
        BoardGame,

        // Character
        Lead = 100,
        Cook,
        Bell,
        DrK,
    }

    public class ItemManager : IService
    {
        private readonly Dictionary<EItem, IItemEffect> _itemEffects = new();
        public Dictionary<EItem, float> Items { get; private set; } = new();

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

        public async UniTask InitializeAsync()
        {
            // ItemEffects 초기화
            var data = await ResourceManager.LoadAssetAsync<ItemEffectData>();

            foreach (var itemEffect in data.ItemEffects)
            {
                if (!_itemEffects.ContainsKey(itemEffect.Item))
                {
                    _itemEffects.Add(itemEffect.Item, itemEffect);
                }
            } 

            // Temp
            AddItem(EItem.Lead);
            AddItem(EItem.Cook);
            AddItem(EItem.DrK);
            AddItem(EItem.Bell);

            AddItem(EItem.Food, 4f);
            AddItem(EItem.Water, 3f);
        }

        public void Dipose()
        {
            Items.Clear();
        }

        public void AddItem(EItem item, float count = 1f)
        {
            if (Items.TryGetValue(item, out var remain))
            {
                Items[item] = remain + count;
            }
            else
            {
                Items.Add(item, count);
            }
        }

        public void RemoveItem(EItem item)
        {
            if (Items.ContainsKey(item))
            {
                Items.Remove(item);
            }
        }

        public void UsedItem(CharacterBase target, EItem item, float usedCount)
        {
            if (Items.TryGetValue(item, out var remain))
            {
                if (remain - usedCount > 0)
                {
                    Debug.Log("남은 Item 갯수를 초과하였습니다.");
                    return;
                }

                if (_itemEffects.TryGetValue(item, out var itemEffect))
                {
                    itemEffect.Apply(target);
                    Items[item] -= usedCount;

                    if (Items[item] == 0f)
                    {
                        RemoveItem(item);
                    }
                }
                else
                {
                    Debug.LogWarning($"ItemEffect [{itemEffect}] not found.");
                }
            }
            else
            {
                Debug.LogWarning($"Item [{item}] not found.");
                return;
            }
        }

        public bool HasItem(EItem item)
        {
            return Items.ContainsKey(item);
        }

        public float GetItemCount(EItem item)
        {
            if (Items.TryGetValue(item, out float count))
            {
                return count;
            }

            return 0.0f;
        }
    }
}
