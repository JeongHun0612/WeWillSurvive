using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        #region UI
        public UI_Scene SceneUI;
        public Stack<UI_Popup> PopUIStack = new Stack<UI_Popup>();

        // Popup UI 초기화
        public void CloseAllPopupUI()
        {
            while (true)
            {
                if (PopUIStack.Count == 0) break;
                UI_Popup ui = PopUIStack.Pop();
                if (ui != null)
                    Destroy(ui.gameObject);
            }
        }

        // 제일 위에 있는 Popup UI 닫기
        public void ClosePopupUI()
        {
            if (PopUIStack.Count == 0) return;
            UI_Popup ui = PopUIStack.Pop();
            if (ui != null)
                Destroy(ui.gameObject);
        }
        #endregion

        #region Item
        float[] _itemCount = new float[(int)EItem.MaxCount];

        public float GetItemCount(EItem type)
        {
            return _itemCount[(int)type];
        }

        // 남은 아이템 확인하고 호출해야함
        public void UseItem(EItem type, float count)
        {
            float remain = _itemCount[(int)type];
            if (remain < count) return;

            _itemCount[(int)type] = remain - count;
        }

        // 파밍 시스템에서 출구에 넣었을 때 / 이벤트로 아이템 얻었을 때 
        public void GetItem(EItem type, float count)
        {
            _itemCount[(int)type] += count;
        }

        #endregion

        private void Start()
        {
            ServiceLocator.AutoRegisterServices();

            if (SceneManager.GetActiveScene().name == "2D")
                ServiceLocator.Get<ResourceService>().LoadAsset("UI_Background").ContinueWith(prefab => Instantiate(prefab)).Forget();
        }
    }
}
