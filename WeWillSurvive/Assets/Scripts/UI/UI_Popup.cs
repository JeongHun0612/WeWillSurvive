using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class UI_Popup : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            // Canvas ¼³Á¤
            Canvas canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10 + GameManager.Instance.PopUIStack.Count;

            GameManager.Instance.PopUIStack.Push(this);
        }

        public void ClosePopupUI()
        {
            GameManager.Instance.ClosePopupUI();
        }
    }
}