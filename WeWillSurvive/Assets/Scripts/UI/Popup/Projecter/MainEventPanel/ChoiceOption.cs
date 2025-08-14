using System;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Util;

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

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();
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
            UpdateChoiceButtonState(eventChoice);
        }

        public void Disabeld()
        {
            _eventChoice = null;
            _choiceOptionIconData = null;
            gameObject.SetActive(false);
        }

        public void OnSelected(bool isSelected)
        {
            if (isSelected)
                _image.sprite = _choiceOptionIconData.NormalSprite;
            else
                _image.sprite = _choiceOptionIconData.DisabledSprite;
        }

        private void UpdateChoiceButtonState(EventChoice eventChoice)
        {
            bool isAvailable = false;

            if (Enum.TryParse($"{eventChoice.ChoiceIcon}", out ECharacter characterType))
            {
                var character = CharacterManager.GetCharacter(characterType);
                isAvailable = (character != null && character.IsInShelter);
            }
            else if (Enum.TryParse($"{eventChoice.ChoiceIcon}", out EItem item))
            {
                if (eventChoice.Amount == 0)
                    isAvailable = true;
                else
                    isAvailable = ItemManager.HasItem(item, eventChoice.Amount);
            }
            else
            {
                isAvailable = true;
            }

            _button.interactable = isAvailable;
            _notingIcon.SetActive(!isAvailable);
        }
    }
}
