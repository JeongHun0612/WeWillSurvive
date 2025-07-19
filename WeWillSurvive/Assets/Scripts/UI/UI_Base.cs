using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeWillSurvive.UI
{
    public class UI_Base : MonoBehaviour
    {
        protected Canvas _canvas;

        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        public virtual void OnShow()
        {
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            OnHide();
        }

        public virtual void OnHide()
        {
        }

        public virtual void Initialize() { }

        public virtual async UniTask InitializeAsync() { await UniTask.Yield(); }

        public virtual void CanvasInitialize()
        {
            // Canvas ¼³Á¤
            _canvas = GetComponent<Canvas>();
            if (_canvas == null)
            {
                Debug.LogError($"{name} Canvas Component is Null");
                return;
            }

            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.overrideSorting = true;
        }
    }
}
