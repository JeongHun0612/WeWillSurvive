using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.ItemEffect;

namespace WeWillSurvive.Item
{
    public enum EItem
    {
        [InspectorName("우주식량")] [Description("우주식량")] Food,
        [InspectorName("특별우주식량")] [Description("특별우주식량")] SpecialFood,
        [InspectorName("물")] [Description("물")] Water,
        [InspectorName("의료키트")] [Description("의료키트")] MedicKit,
        [InspectorName("특별의료키트")] [Description("특별의료키트")] SpecialMedicKit,
        [InspectorName("수리키트")] [Description("수리키트")] RepairKit,
        [InspectorName("특별수리키트")] [Description("특별수리키트")] SpecialRepairKit,
        [InspectorName("예비통신장비")] [Description("예비통신장비")] CommDevice,
        [InspectorName("고급우주복")] [Description("고급우주복")] NiceSpacesuit,
        [InspectorName("총")] [Description("총")] Gun,
        [InspectorName("보드게임")] [Description("보드게임")] BoardGame,
        [InspectorName("도끼")] [Description("도끼")] Ax,
        [InspectorName("쇠파이프")] [Description("쇠파이프")] Pipe,
        [InspectorName("손전등")] [Description("손전등")] Flashlight,
        [InspectorName("행성탐사지도")] [Description("행성탐사지도")] Map,

        // Character
        Lead = 100,
        Cook,
        Bell,
        DrK,
        Starmac,
    }

    public class ItemManager : IService
    {
        private readonly Dictionary<EItem, ScriptableItemEffect> _itemEffects = new();
        public Dictionary<EItem, float> Items { get; private set; } = new();

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

        public async UniTask InitializeAsync()
        {
            // ItemEffects 초기화
            var itemEffects = await ResourceManager.LoadAssetsByLabelAsync<ScriptableItemEffect>("ItemEffect");
            foreach (var itemEffect in itemEffects)
            {
                if (!_itemEffects.ContainsKey(itemEffect.ItemType))
                {
                    _itemEffects.Add(itemEffect.ItemType, itemEffect);
                }
            }

            // Temp
            AddItem(EItem.Lead);
            AddItem(EItem.Cook);
            AddItem(EItem.DrK);
            //AddItem(EItem.Bell);

            AddItem(EItem.Food, 5.25f);
            AddItem(EItem.Water, 5.25f);
            AddItem(EItem.BoardGame);
            AddItem(EItem.Gun);
            AddItem(EItem.MedicKit);
            AddItem(EItem.RepairKit);
            AddItem(EItem.NiceSpacesuit);
            AddItem(EItem.CommDevice);
            AddItem(EItem.Ax);
            AddItem(EItem.Pipe);
            AddItem(EItem.Flashlight);
            AddItem(EItem.Map);
            AddItem(EItem.Starmac);
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
                if (remain - usedCount < 0)
                {
                    Debug.Log("남은 Item 갯수를 초과하였습니다.");
                    return;
                }

                if (_itemEffects.TryGetValue(item, out var itemEffect))
                {
                    itemEffect.Apply(target);
                }

                Items[item] -= usedCount;
                if (Items[item] == 0f)
                {
                    RemoveItem(item);
                }
            }
            else
            {
                Debug.LogWarning($"Item [{item}] not found.");
                return;
            }
        }

        public void UpdateItemCount(EItem item, float updateCount)
        {
            if (!Items.ContainsKey(item))
            {
                Debug.LogWarning($"Item [{item}] not found.");
                return;
            }

            Items[item] += updateCount;
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

        public float GetSupportItemsCount()
        {
            return Items
                .Where(kvp => kvp.Key != EItem.Water && kvp.Key != EItem.Food)
                .Sum(kvp => kvp.Value);
        }
    }
}
