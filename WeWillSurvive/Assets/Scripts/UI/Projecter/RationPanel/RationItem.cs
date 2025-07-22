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

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public event Action ItemSelectedEvent;

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

        public void UsedItem(CharacterBase target)
        {
            if (_isSelected)
            {
                ItemManager.UsedItem(target, _item, 0f);
            }
        }

        private void SetItemState(bool isSelected)
        {
            if (isSelected)
            {
                _itemImage.sprite = _selectedSprite;
                ItemManager.UpdateItemCount(_item, -_usageAmount);
            }
            else
            {
                _itemImage.sprite = _normalSprite;
                ItemManager.UpdateItemCount(_item, _usageAmount);
            }
        }

        public void OnClickRationItem()
        {
            if (!_isSelected)
            {
                var itemCount = ItemManager.GetItemCount(_item);
                if (itemCount < _usageAmount)
                {
                    Debug.LogWarning($"아이템 수량이 부족합니다. {itemCount}");
                    return;
                }
            }

            _isSelected = !_isSelected;
            SetItemState(_isSelected);

            ItemSelectedEvent?.Invoke();
        }
    }
}
