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

        public EItem Item => _item;
        public float UsageAmount => _usageAmount;
        public bool IsSelected => _isSelected;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public void Initialize()
        {
            _itemImage = gameObject.GetComponent<Image>();
            _itemButton = gameObject.GetComponent<Button>();

            Refresh();
        }

        public void RegisterEvent(Action<RationItem> callback)
        {
            if (_itemButton == null)
            {
                Debug.LogWarning($"[{name}] Button Component is null");
                return;
            }

            _itemButton.onClick.RemoveAllListeners();
            _itemButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX(ESFX.SFX_Click_1);
                callback?.Invoke(this);
            });
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
                ItemManager.UsedItem(_item, _usageAmount, target);
                _isSelected = false;
            }
        }

        public void OnSelected(bool isSelected)
        {
            if (_isSelected == isSelected)
                return;

            UpdateSprite(isSelected);

            _isSelected = isSelected;
        }

        public void UpdateSprite(bool isSelected)
        {
            _itemImage.sprite = isSelected ? _selectedSprite : _normalSprite;
        }
    }
}