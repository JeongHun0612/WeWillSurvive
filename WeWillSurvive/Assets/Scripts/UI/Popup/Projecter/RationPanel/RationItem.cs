using System;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class RationItem : MonoBehaviour
    {
        [SerializeField] private EItem _item;
        [SerializeField] private float _usageAmount;

        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _selectedSprite;

        private Image _itemImage;
        private Button _itemButton;

        private bool _isSelected;

        public event Action<RationItem> ItemSelectedEvent;

        public bool IsSelected => _isSelected;
        public float UsageAmount => _usageAmount;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public void Initialize()
        {
            _itemImage = gameObject.GetComponent<Image>();

            _itemButton = gameObject.GetComponent<Button>();
            _itemButton.onClick.AddListener(OnClickRationItem);

            Refresh();
        }

        public void Refresh()
        {
            _isSelected = false;
            _itemImage.sprite = _normalSprite;
        }

        public void UsedItem(CharacterBase target = null)
        {
            if (_isSelected)
            {
                ItemManager.UsedItem(_item, 0f, target);
                _isSelected = false;
            }
        }

        public void OnSelected(bool isSelected, bool affectInventory = true)
        {
            if (_isSelected == isSelected)
                return;

            if (affectInventory)
            {
                // 사용량 증감
                float itemCount = ItemManager.GetItemCount(_item);
                float updateItemCount = isSelected
                    ? itemCount - _usageAmount
                    : itemCount + _usageAmount;

                ItemManager.UpdateItemCount(_item, updateItemCount);
            }

            // Sprite 갱신
            _itemImage.sprite = isSelected ? _selectedSprite : _normalSprite;

            // 상태 갱신
            _isSelected = isSelected;
        }

        public void SetItemState(bool isSelected)
        {
            if (_isSelected == isSelected)
                return;

            float updateItemCount = ItemManager.GetItemCount(_item)
                        + (isSelected ? -_usageAmount : _usageAmount);

            ItemManager.UpdateItemCount(_item, updateItemCount);
        }

        public void OnClickRationItem()
        {
            var itemCount = ItemManager.GetItemCount(_item);
            if (!_isSelected && itemCount < _usageAmount)
            {
                Debug.LogWarning($"아이템 수량이 부족합니다. {itemCount}");
                return;
            }

            //OnSelected(!_isSelected);
            ItemSelectedEvent?.Invoke(this);
            Debug.Log($"[{_item}] Count: {ItemManager.GetItemCount(_item)}");

            //_isSelected = !_isSelected;
            //OnSelected(_isSelected);
            //ItemSelectedEvent?.Invoke(this);
            //Debug.Log($"[{_item}] Count: {ItemManager.GetItemCount(_item)}");
        }
    }
}