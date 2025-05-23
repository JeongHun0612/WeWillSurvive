using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive.UI
{
    public class UI_Popup : UI_Base
    {
        public bool RememberInHistory = true;

        public override void Initialize()
        {
            base.Initialize();

            // Canvas ¼³Á¤
            Canvas canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10 + UIManager.Instance.PopupHistoryCount;
        }

        public void ClosePopupUI()
        {
            UIManager.Instance.CloseCurrentPopup();
        }
    }
}