using System;
using UnityEngine;
using UnityEngine.UI;

namespace WeWillSurvive
{
    [RequireComponent(typeof(Button))]
    public class PanelMoveButton : MonoBehaviour
    {
        [SerializeField] private EPanelType _panelType;
        [SerializeField] private Color _selectedColor;

        private Color _normalColor;
        private Button _button;

        public EPanelType PanelType => _panelType;

        public void Initialize(Action<EPanelType> callback)
        {
            _button = gameObject.GetComponent<Button>();
            _button.onClick.AddListener(() => callback(_panelType));

            _normalColor = _button.image.color;
        }

        public void Enabled() => _button.interactable = true;
        public void Disabled() => _button.interactable = false;

        public void OnSelected(bool isSelected)
        {
            if (isSelected)
                _button.image.color = _selectedColor;
            else
                _button.image.color = _normalColor;
        }
    }
}
