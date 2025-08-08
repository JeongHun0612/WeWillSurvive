using UnityEngine;
using WeWillSurvive.UI;
using UnityEngine.UI;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class ShowStatus : MonoBehaviour
    {
        [SerializeField] private EItem _itemType;
        [SerializeField] private Vector2 _statusPanelPosition = Vector2.zero;

        public void ShowStatusPanel(string text)
        {
            UIManager.Instance.ClosePopups(remain: 1);
            UI_StatePanel ui = UIManager.Instance.ShowPopup<UI_StatePanel>();

            ui.ShowStatePanel(_statusPanelPosition, text);
        }
    }
}
