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

        public EventChoice EventChoice => _eventChoice;

        public void Initialize(Action<ChoiceOption> callback)
        {
            _image = gameObject.GetComponent<Image>();
            _button = gameObject.GetComponent<Button>();

            _button.onClick.AddListener(() => callback?.Invoke(this));
        }

        public void InitChoiceData(EventChoice eventChoice)
        {
            _eventChoice = eventChoice;

            SetChoiceIcon(eventChoice.iconTexture);

            gameObject.SetActive(true);
        }

        public void Initialize(EventChoice eventChoice)
        {
            _eventChoice = eventChoice;

            SetChoiceIcon(eventChoice.iconTexture);

            gameObject.SetActive(true);
        }

        public void SetChoiceIcon(Texture2D iconTexture)
        {
            _image.sprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f));
        }

        public void OnSelected(bool isSelected)
        {
            if (isSelected)
                _image.color = Color.white;
            else
                _image.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }
}
