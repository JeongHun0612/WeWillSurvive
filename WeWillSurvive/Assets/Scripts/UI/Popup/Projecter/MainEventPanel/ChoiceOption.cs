using System;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.GameEvent;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class ChoiceOption : MonoBehaviour
    {
        [SerializeField] private GameObject _notingIcon;

        private Image _image;
        private Button _button;

        private EventChoice _eventChoice;

        private Sprite _normalSprite;
        private Sprite _disabledSprite;

        private bool _isSelected;
        private float _remainItemCount;

        public EventChoice EventChoice => _eventChoice;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Initialize(Action<ChoiceOption> callback)
        {
            _image = gameObject.GetComponent<Image>();
            _button = gameObject.GetComponent<Button>();

            _button.onClick.AddListener(() => callback?.Invoke(this));
        }

        public void ResetState()
        {
            _eventChoice = null;
            _normalSprite = null;
            _disabledSprite = null;

            _isSelected = false;
            _remainItemCount = 0f;
            gameObject.SetActive(false);
        }

        public void UpdateChoiceOption(EventChoice eventChoice)
        {
            if (eventChoice == null)
            {
                Debug.LogWarning("EventChoice 데이터가 null입니다.");
                return;
            }

            _eventChoice = eventChoice;

            var choiceIconData = GameEventUtil.GetChoiceIconData(eventChoice.ChoiceIcon);
            _normalSprite = choiceIconData.NormalSprite;
            _disabledSprite = choiceIconData.DisabledSprite;

            if (Enum.TryParse($"{eventChoice.ChoiceIcon}", out EItem item))
            {
                _remainItemCount = ItemManager.GetItemCount(item);
            }

            UpdateChoiceSprite();
            UpdateAvailable();
            gameObject.SetActive(true);
        }

        public void UpdateAvailable()
        {
            if (_isSelected)
                return;

            bool isAvailable = GetIsAvailable();
            _button.interactable = isAvailable;
            _notingIcon.SetActive(!isAvailable);
        }

        public void UpdateRemainItemCount(float updateCount) => _remainItemCount = updateCount;

        public void OnSelected(bool isSelected)
        {
            if (_isSelected == isSelected)
                return;

            _isSelected = isSelected;

            UpdateChoiceSprite();

            if (Enum.TryParse($"{_eventChoice.ChoiceIcon}", out EItem item) && ItemManager.IsRationItem(item))
            {
                // 이벤트 발생
                EventBus.Publish(new ChoiceOptionSelectedEvent
                {
                    Item = item,
                    IsSelected = isSelected,
                    RequiredAmount = _eventChoice.RequiredAmount
                });
            }
        }

        private void UpdateChoiceSprite()
        {
            _image.sprite = (_isSelected) ? _normalSprite : _disabledSprite;
        }

        private bool GetIsAvailable()
        {
            if (_eventChoice == null)
                return false;

            if (Enum.TryParse<ECharacter>($"{_eventChoice.ChoiceIcon}", out var characterType))
            {
                var character = CharacterManager.GetCharacter(characterType);
                return (character != null && character.IsInShelter);
            }
            else if (Enum.TryParse<EItem>($"{_eventChoice.ChoiceIcon}", out var item) && ItemManager.IsRationItem(item))
            {
                return _eventChoice.RequiredAmount == 0 || _remainItemCount >= _eventChoice.RequiredAmount;
            }

            return true;
        }
    }
}
