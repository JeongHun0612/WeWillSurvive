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
            _canvasGroup.alpha = 1f; // ���̰� (������)
            _canvasGroup.interactable = true; // ���콺/��ġ �� ��ȣ�ۿ� ����
            _canvasGroup.blocksRaycasts = true; // UI ���� �ٸ� ��� Ŭ�� ����
        }

        public void HidePanel()
        {
            _canvasGroup.alpha = 0f; // �� ���̰� (����)
            _canvasGroup.interactable = false; // ��ȣ�ۿ� �Ұ���
            _canvasGroup.blocksRaycasts = false; // UI ���� �ٸ� ��� Ŭ�� ����
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