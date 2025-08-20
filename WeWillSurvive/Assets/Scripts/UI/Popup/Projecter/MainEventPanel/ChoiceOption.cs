using System;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.GameEvent;

namespace WeWillSurvive
{
    public class ChoiceOption : MonoBehaviour
    {
        [SerializeField] private GameObject _notingIcon;

        private Image _image;
        private Button _button;

        private EventChoice _eventChoice;
        private ChoiceIconData _choiceIconData;

        public EventChoice EventChoice => _eventChoice;

        public void Initialize(Action<ChoiceOption> callback)
        {
            _image = gameObject.GetComponent<Image>();
            _button = gameObject.GetComponent<Button>();

            _button.onClick.AddListener(() => callback?.Invoke(this));
        }

        public void UpdateChoiceOption(EventChoice eventChoice)
        {
            _eventChoice = eventChoice;
            _choiceIconData = GameEventUtil.GetChoiceIconData(eventChoice.ChoiceIcon);

            bool isAvailable = GameEventUtil.IsAvailable(eventChoice);
            _button.interactable = isAvailable;
            _notingIcon.SetActive(!isAvailable);

            OnSelected(false);

            gameObject.SetActive(true);
        }

        public void Disabeld()
        {
            _eventChoice = null;
            _choiceIconData = null;
            gameObject.SetActive(false);
        }

        public void OnSelected(bool isSelected)
        {
            if (isSelected)
                _image.sprite = _choiceIconData?.NormalSprite;
            else
                _image.sprite = _choiceIconData?.DisabledSprite;
        }
    }
}
