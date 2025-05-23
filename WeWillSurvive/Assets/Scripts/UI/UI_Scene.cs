using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive.UI
{
    public class UI_Scene : UI_Base
    {
        private void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            // Scene UI �ʱ�ȭ
            UI_Scene sceneUI = GameManager.Instance.SceneUI;
            if (sceneUI != null)
            {
                Destroy(sceneUI.gameObject);
                sceneUI = null;
            }
            GameManager.Instance.SceneUI = this;

            // Canvas ����
            Canvas canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
        }
    }
}