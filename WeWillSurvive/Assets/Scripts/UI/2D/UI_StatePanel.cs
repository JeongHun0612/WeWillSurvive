using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_StatePanel : UI_Popup
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private TMP_Text _stateText;

        public void SetPanel(Vector2 panelPos, string text)
        {
            _panel.localPosition = panelPos;
            _stateText.text = text;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);
        }
    }
}
