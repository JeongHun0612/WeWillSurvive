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
        [Header("캐릭터 여부")]
        public ItemData Lead = new ItemData(EItem.Lead);
        public ItemData Cook = new ItemData(EItem.Cook);
        public ItemData Bell = new ItemData(EItem.Bell);
        public ItemData DrK = new ItemData(EItem.DrK);

        [Header("아이템 여부")]
        public ItemData Food = new ItemData(EItem.Food);
        public ItemData Water = new ItemData(EItem.Water);
        public ItemData SpecialFood = new ItemData(EItem.SpecialFood);
        public ItemData MedicKit = new ItemData(EItem.MedicKit);
        public ItemData SpecialMedicKit = new ItemData(EItem.SpecialMedicKit);
        public ItemData RepairKit = new ItemData(EItem.RepairKit);
        public ItemData SpecialRepairKit = new ItemData(EItem.SpecialRepairKit);
        public ItemData CommDevice = new ItemData(EItem.CommDevice);
        public ItemData NiceSpacesuit = new ItemData(EItem.NiceSpacesuit);
        public ItemData Gun = new ItemData(EItem.Gun);
        public ItemData BoardGame = new ItemData(EItem.BoardGame);
        public ItemData Ax = new ItemData(EItem.Ax);
        public ItemData Pipe = new ItemData(EItem.Pipe);
        public ItemData Flashlight = new ItemData(EItem.Flashlight);
        public ItemData Map = new ItemData(EItem.Map);

        public List<ItemData> GetItemDatas()
        {
            return this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(field => field.FieldType == typeof(ItemData))
                .Select(field => (ItemData)field.GetValue(this))
                .ToList();
        }
    }
}
