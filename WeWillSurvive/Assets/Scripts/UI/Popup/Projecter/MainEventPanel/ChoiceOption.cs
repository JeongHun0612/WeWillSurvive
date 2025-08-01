using System;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive
{
    public class ChoiceOption : MonoBehaviour
    {
        private Image _image;
        private Button _button;

        private EventChoice _eventChoice;
        private ChoiceOptionIconData _choiceOptionIconData;

        public EventChoice EventChoice => _eventChoice;

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
