using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeWillSurvive
{
    public class StatePanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private TMP_Text _stateText;

        public void ShowPanel()
        {
            _canvasGroup.alpha = 1f; // 보이게 (불투명)
            _canvasGroup.interactable = true; // 마우스/터치 등 상호작용 가능
            _canvasGroup.blocksRaycasts = true; // UI 뒤의 다른 요소 클릭 방지
        }

        public void HidePanel()
        {
            _canvasGroup.alpha = 0f; // 안 보이게 (투명)
            _canvasGroup.interactable = false; // 상호작용 불가능
            _canvasGroup.blocksRaycasts = false; // UI 뒤의 다른 요소 클릭 가능
        }

        public void MovePanelPosition(Vector2 panelPos)
        {
            _panel.localPosition = panelPos;
        }

        public void SetStateText(string text)
        {
            _stateText.text = text;
            _panel.sizeDelta = new Vector2(_stateText.preferredWidth, _panel.sizeDelta.y);

            _stateText.ForceMeshUpdate();
            _panel.sizeDelta = new Vector2(_panel.sizeDelta.x, _stateText.preferredHeight);
        }
    }
}