using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    [CreateAssetMenu(fileName = "ItemDebugData", menuName = "Scriptable Objects/Debug/ItemDebugData")]
    public class ItemDebugData : ScriptableObject
    {
        [System.Serializable]
        public class ItemData
        {
            public EItem item;
            public bool isActive;
            public float count;
        }

        [Header("캐릭터 여부")]
        public ItemData Lead = new ItemData() { item = EItem.Lead, isActive = false };
        public ItemData Cook = new ItemData() { item = EItem.Cook, isActive = false };
        public ItemData Bell = new ItemData() { item = EItem.Bell, isActive = false };
        public ItemData DrK = new ItemData() { item = EItem.DrK, isActive = false };

        [Header("아이템 여부")]
        public ItemData Food = new ItemData() { item = EItem.Food, isActive = false, count = 1f};
        public ItemData Water = new ItemData() { item = EItem.Water, isActive = false, count = 1f};
        public ItemData SpecialFood = new ItemData() { item = EItem.SpecialFood, isActive = false, count = 1f};
        public ItemData MedicKit = new ItemData() { item = EItem.MedicKit, isActive = false, count = 1f};
        public ItemData SpecialMedicKit = new ItemData() { item = EItem.SpecialMedicKit, isActive = false, count = 1f};
        public ItemData RepairKit = new ItemData() { item = EItem.RepairKit, isActive = false, count = 1f};
        public ItemData SpecialRepairKit = new ItemData() { item = EItem.SpecialRepairKit, isActive = false, count = 1f};
        public ItemData CommDevice = new ItemData() { item = EItem.CommDevice, isActive = false, count = 1f};
        public ItemData NiceSpacesuit = new ItemData() { item = EItem.NiceSpacesuit, isActive = false, count = 1f};
        public ItemData Gun = new ItemData() { item = EItem.Gun, isActive = false, count = 1f};
        public ItemData BoardGame = new ItemData() { item = EItem.BoardGame, isActive = false, count = 1f};
        public ItemData Ax = new ItemData() { item = EItem.Ax, isActive = false, count = 1f};
        public ItemData Pipe = new ItemData() { item = EItem.Pipe, isActive = false, count = 1f};
        public ItemData Flashlight = new ItemData() { item = EItem.Flashlight, isActive = false, count = 1f};
        public ItemData Map = new ItemData() { item = EItem.Map, isActive = false, count = 1f};

        public List<ItemData> GetItemDatas()
        {
            return this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(field => field.FieldType == typeof(ItemData))
                .Select(field => (ItemData)field.GetValue(this))
                .ToList();
        }
    }
}
