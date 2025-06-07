using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeWillSurvive.UI
{
    public class UI_Base : MonoBehaviour
    {
        protected bool _initialized = false;

        private void Start()
        {
            if (_initialized) return;
            Initialize();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);

            if (!_initialized) Initialize();
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

        public virtual void Initialize() { _initialized = true; }

        public virtual async UniTask InitializeAsync() { await UniTask.Yield(); }
    }
}
