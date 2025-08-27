using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_DebugSetting : UI_Popup
    {
        [SerializeField] private List<ItemToggle> _itemToggles = new();

        private List<ItemData> _itemDatas = new();

        private DataManager DataManager => ServiceLocator.Get<DataManager>();

        public async override UniTask InitializeAsync()
        {
            _itemDatas = DataManager.LoadDataList<ItemData>();

            foreach (var itemToggle in _itemToggles)
            {
                var itemData = _itemDatas.FirstOrDefault(itemData => itemData.Item == itemToggle.Item);
                itemToggle.Initialize(itemData);
            }

            await UniTask.Yield();
        }

        public void OnClickSave()
        {
            foreach (var itemToggle in _itemToggles)
            {
                var itemData = _itemDatas.FirstOrDefault(itemData => itemData.Item == itemToggle.Item);
                if (itemData == null)
                {
                    _itemDatas.Add(new ItemData(itemToggle.Item, itemToggle.IsActive, itemToggle.Count));
                }
                else
                {
                    itemData.IsActive = itemToggle.IsActive;
                    itemData.Count = itemToggle.Count;
                }
            }

            DataManager.SaveDataList(_itemDatas);
            UIManager.Instance.CloseCurrentPopup();
        }

        public void OnClickExit()
        {
            UIManager.Instance.CloseCurrentPopup();
        }
    }

    [System.Serializable]
    public class ItemData
    {
        public EItem Item;
        public bool IsActive;
        public float Count;

        public ItemData(EItem item, bool isActive = false, float count = 0f)
        {
            Item = item;
            IsActive = isActive;
            Count = count;
        }
    }
}
