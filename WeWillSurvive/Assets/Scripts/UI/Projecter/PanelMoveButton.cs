using System;
using UnityEngine;
using UnityEngine.UI;

namespace WeWillSurvive
{
    [RequireComponent(typeof(Button))]
    public class PanelMoveButton : MonoBehaviour
    {
        [SerializeField] private EPanelType _panelType;
        [SerializeField] private GameObject _disabledImage;

        private Button _button;

        public EPanelType PanelType => _panelType;

        public void Initialize(Action<EPanelType> callback)
        {
            _button = gameObject.GetComponent<Button>();
            _button.onClick.AddListener(() => callback(_panelType));

            _disabledImage.SetActive(false);
        }

        public void Enabled()
        {
            _button.interactable = true;
            _disabledImage.SetActive(false);
        }

        public void Disabled()
        {
            _button.interactable = false;
            _disabledImage.SetActive(true);
        }

        public void OnSelected(bool isSelected)
        {
            if (!_button.interactable) return;

            if (isSelected)
                _button.image.color = Color.white;
            else
                _button.image.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
    }
}
