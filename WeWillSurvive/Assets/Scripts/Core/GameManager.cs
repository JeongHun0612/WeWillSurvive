using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeWillSurvive.UI;
using static Define;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private async void Start()
        {
            ServiceLocator.AutoRegisterServices();

            await UIManager.Instance.InitializeAsync();

            if (SceneManager.GetActiveScene().name == "2D")
                ServiceLocator.Get<ResourceService>().LoadAsset("UI_Background").ContinueWith(prefab => Instantiate(prefab)).Forget();
        }

        #region UI
        public UI_Scene SceneUI;
        public Stack<UI_Popup> PopUIStack = new Stack<UI_Popup>();

        // Popup UI �ʱ�ȭ
        public void CloseAllPopupUI()
        {
            ClosePopupUIs(remain: 0);
        }

        /// <param name="remain"> ���� �ʰ� ���ܵ� Popup UI ���� </param>
        public void ClosePopupUIs(int remain)
        {
            while (true)
            {
                if (PopUIStack.Count <= remain) break;
                UI_Popup ui = PopUIStack.Pop();
                if (ui != null)
                    Destroy(ui.gameObject);
            }
        }

        // ���� ���� �ִ� Popup UI �ݱ�
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

        // ���� ������ Ȯ���ϰ� ȣ���ؾ���
        public void UseItem(EItem type, float count)
        {
            float remain = _itemCount[(int)type];
            if (remain < count) return;

            _itemCount[(int)type] = remain - count;
        }

        // �Ĺ� �ý��ۿ��� �ⱸ�� �־��� �� / �̺�Ʈ�� ������ ����� �� 
        public void GetItem(EItem type, float count)
        {
            _itemCount[(int)type] += count;
        }

        #endregion
    }
}
