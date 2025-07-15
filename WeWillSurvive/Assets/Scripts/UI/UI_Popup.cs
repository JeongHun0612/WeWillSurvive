using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeWillSurvive.UI
{
    public class UI_Popup : UI_Base
    {
        public bool RememberInHistory = true;

        private Canvas _canvas;

        public override void Initialize()
        {
            base.Initialize();

            // Canvas 설정
            _canvas = GetComponent<Canvas>();
            if (_canvas == null)
            {
                Debug.LogError($"{name} Canvas Component is Null");
                return;
            }

            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.overrideSorting = true;
        }

        public override void OnShow()
        {
            SetCanvasOrder();
        }

        private void SetCanvasOrder()
        {
            if (_canvas == null)
            {
                Debug.LogError($"Canvas is Null");
                return;
            }

            _canvas.sortingOrder = 10 + UIManager.Instance.PopupHistoryCount;
        }

        public void ClosePopupUI()
        {
            UIManager.Instance.CloseCurrentPopup();
        }
    }
}