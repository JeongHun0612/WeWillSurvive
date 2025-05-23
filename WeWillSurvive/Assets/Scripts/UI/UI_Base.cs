using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeWillSurvive.UI
{
    public class UI_Base : MonoBehaviour
    {
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
    }
}
