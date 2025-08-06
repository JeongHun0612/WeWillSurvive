using System;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive
{
    public class ChoiceOption : MonoBehaviour
    {
        [SerializeField] private GameObject _notingIcon;

        private Image _image;
        private Button _button;

        private EventChoice _eventChoice;
        private ChoiceOptionIconData _choiceOptionIconData;

        public EventChoice EventChoice => _eventChoice;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public void Initialize(Action<ChoiceOption> callback)
        {
            _image = gameObject.GetComponent<Image>();
            _button = gameObject.GetComponent<Button>();

            _button.onClick.AddListener(() => callback?.Invoke(this));
        }

        public void Initialize(EventChoice eventChoice, ChoiceOptionIconData choiceOptionIconData)
        {
            _eventChoice = eventChoice;
            _choiceOptionIconData = choiceOptionIconData;
            gameObject.SetActive(true);

            OnSelected(false);

            // ChoiceType이 Item이면
            if (Enum.TryParse<EItem>($"{eventChoice.choiceType}", out EItem item))
            {
                bool hasItem = ItemManager.HasItem(item);
                _button.interactable = hasItem;
                _notingIcon.SetActive(!hasItem);
            }
            else
            {
                _button.interactable = true;
                _notingIcon.SetActive(false);
            }
        }

        public void Disabeld()
        {
            _eventChoice = null;
            _choiceOptionIconData = null;
            gameObject.SetActive(false);
        }

        public void OnSelected(bool isSelected)
        {
            if (_choiceOptionIconData == null)
                return;

            if (isSelected)
                _image.sprite = _choiceOptionIconData.NormalSprite;
            else
                _image.sprite = _choiceOptionIconData.DisabledSprite;
        }
    }
}
