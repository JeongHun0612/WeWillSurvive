using Cysharp.Threading.Tasks;
using UnityEngine;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_StatePanel : UI_Popup
    {
        [SerializeField] private StatePanel _statePanel;

        public async override UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }

        public void ShowStatePanel(Vector2 panelPos, string text)
        {
            _statePanel.MovePanelPosition(panelPos);
            _statePanel.SetStateText(text);

            _statePanel.ShowPanel();
        }
    }
}
