using System.Collections.Generic;
using UnityEngine;

namespace WeWillSurvive
{
    [System.Serializable]
    public class ItemInfo
    {
        public string itemName;
        public int id;
        public float hp;
    }

    [CreateAssetMenu(fileName = "ItemList", menuName = "WeWillSurvive/ItemList")]
    public class ItemList : ScriptableObject
    {
        public List<ItemInfo> items = new List<ItemInfo>();
    }
}