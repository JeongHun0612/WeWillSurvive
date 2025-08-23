using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeWillSurvive.UI
{
    public class UI_Popup : UI_Base
    {
        private const int DEFAULT_SORTING_ORDER = 20;

        public bool RememberInHistory = true;

        public override void OnShow()
        {
            SetCanvasOrder();
        }

        public override void CanvasInitialize()
        {
            base.CanvasInitialize();

            _canvas.sortingOrder = DEFAULT_SORTING_ORDER;
        }

        private void SetCanvasOrder()
        {
            if (_canvas == null)
                CanvasInitialize();

            _canvas.sortingOrder = DEFAULT_SORTING_ORDER + UIManager.Instance.PopupHistoryCount;
        }

        public void ClosePopupUI()
        {
            UIManager.Instance.CloseCurrentPopup();
        }
    }
}