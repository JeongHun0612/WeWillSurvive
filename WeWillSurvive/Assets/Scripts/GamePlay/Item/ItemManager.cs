using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.ItemEffect;

namespace WeWillSurvive.Item
{
    public enum EItem
    {
        [InspectorName("우주식량")] Food,
        [InspectorName("특별우주식량")] SpecialFood,
        [InspectorName("물")] Water,
        [InspectorName("의료키트")] MedicKit,
        [InspectorName("특별의료키트")] SpecialMedicKit,
        [InspectorName("수리키트")] RepairKit,
        [InspectorName("특별수리키트")] SpecialRepairKit,
        [InspectorName("예비통신장비")] CommDevice,
        [InspectorName("고급우주복")] NiceSpacesuit,
        [InspectorName("총")] Gun,
        [InspectorName("보드게임")] BoardGame,
        [InspectorName("도끼")] Ax,
        [InspectorName("쇠파이프")] Pipe,
        [InspectorName("손전등")] Flashlight,
        [InspectorName("행성탐사지도")] Map,

        // Character
        Lead = 100,
        Cook,
        Bell,
        DrK,
        Starmac,

        None = 500,
    }

    public class ItemManager : IService
    {
        private readonly Dictionary<EItem, IItemEffect> _itemEffects = new();
        private readonly HashSet<EItem> _specialItemTypes = new HashSet<EItem>
        {
            EItem.SpecialMedicKit,
            EItem.SpecialRepairKit
        };

        public Dictionary<EItem, float> Items { get; private set; } = new();

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

        public async UniTask InitializeAsync()
        {
            // ItemEffects 초기화
            SetupItemEffects();

            // Item Deubg 전용
            //#if UNITY_EDITOR
            //            var itemDebugData = await ResourceManager.LoadAssetAsync<ItemDebugData>("ItemDebugData");
            //            foreach (var itemData in itemDebugData.GetItemDatas())
            //            {
            //                if (!itemData.IsActive)
            //                    continue;

            //                AddItem(itemData.Item, itemData.Count);
            //            }
            //#endif

            await UniTask.Yield();
        }

        public void Dipose()
        {
            Items.Clear();
        }

        public void AddItem(EItem item, float count = 1f)
        {
            // Food 와 Water를 제외하고는 1개씩만 소유 가능
            bool isStackable = (item == EItem.Food || item == EItem.Water);

            if (Items.ContainsKey(item))
            {
                if (isStackable)
                {
                    Items[item] += count;
                }
            }
            else
            {
                if (isStackable)
                {
                    Items.Add(item, count);
                }
                else
                {
                    Items.Add(item, 1f);
                }
            }

            Debug.Log($"[아이템 추가] {item} | Total : {Items[item]}개");
        }

        public void DeleteItem(EItem item, float count)
        {
            TryDecreaseItemCount(item, count);
        }

        public void UsedItem(EItem item, float usedCount, CharacterBase target = null)
        {
            if (!Items.TryGetValue(item, out var remain) || remain - usedCount < 0)
            {
                Debug.LogWarning($"아이템 [{item}]이 부족하거나 없습니다. (보유: {remain}, 필요: {usedCount})");
                return;
            }

            if (_itemEffects.TryGetValue(item, out var itemEffect))
            {
                itemEffect.Apply(target);
            }

            TryDecreaseItemCount(item, usedCount);
        }

        public bool TryDecreaseItemCount(EItem item, float amountToDecrease)
        {
            if (Items.TryGetValue(item, out var remain))
            {
                var newRemain = Mathf.Max(0f, remain - amountToDecrease);
                Items[item] = newRemain;
                Debug.Log($"[아이템 수량 변경] {item} | Total : {newRemain}개");

                if (newRemain <= 0f)
                {
                    RemoveItem(item);
                }
                return true;
            }
            else
            {
                Debug.LogWarning($"개수를 줄일 아이템 [{item}]이(가) 없습니다.");
                return false;
            }
        }

        public void RemoveItem(EItem item)
        {
            if (Items.ContainsKey(item))
            {
                Items.Remove(item);
            }
        }

        public void UpdateItemCount(EItem item, float updateCount)
        {
            if (!Items.ContainsKey(item))
            {
                Debug.LogWarning($"Item [{item}] not found.");
                return;
            }

            Items[item] = updateCount;
        }

        public bool HasItem(EItem item, float count = 1f)
        {
            if (Items.TryGetValue(item, out float remain))
            {
                return remain + 1e-5f >= count;
            }

            return false;
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

        public EItem GetRandomSupportItem()
        {
            // 우선순위 1. 물과 식량을 제외한 아이템 리스트 중 랜덤한 아이템
            var normalSupportItems = Items
                .Where(kvp => kvp.Key != EItem.Water && kvp.Key != EItem.Food && !_specialItemTypes.Contains(kvp.Key))
                .Select(kvp => kvp.Key)
                .ToList();

            if (normalSupportItems.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, normalSupportItems.Count);
                return normalSupportItems[randomIndex];
            }


            // 우선순위 2. 스페셜 아이템들 중 랜덤한 아이템
            var specialSupportItems = Items
                .Where(kvp => _specialItemTypes.Contains(kvp.Key))
                .Select(kvp => kvp.Key)
                .ToList();

            if (specialSupportItems.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, specialSupportItems.Count);
                return specialSupportItems[randomIndex];
            }

            // 우선순위 3. 모든 아이템이 없을 경우
            return EItem.None;
        }

        private void SetupItemEffects()
        {
            // ItemEffects 초기화
            var itemEffectTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IItemEffect).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var itemEffectType in itemEffectTypes)
            {
                IItemEffect itemEffect = (IItemEffect)Activator.CreateInstance(itemEffectType);

                if (!_itemEffects.ContainsKey(itemEffect.ItemType))
                {
                    _itemEffects.Add(itemEffect.ItemType, itemEffect);
                }
            }
        }
    }

    [System.Serializable]
    public class RewardItemData
    {
        [SerializeField]
        private EItem _itemType;

        [SerializeField]
        private float _amount;

        public EItem ItemType => _itemType;
        public float Amount => _amount;

        public RewardItemData(EItem itemType, float amount)
        {
            _itemType = itemType;
            _amount = amount;
        }
    }
}
