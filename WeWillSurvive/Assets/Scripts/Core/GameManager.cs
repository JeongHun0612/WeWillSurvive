using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        #region UI
        public UI_Scene SceneUI;
        public Stack<UI_Popup> PopUIStack = new Stack<UI_Popup>();
        #endregion

        private void Start()
        {
            ServiceLocator.AutoRegisterServices();

            if (SceneManager.GetActiveScene().name == "2D")
                ServiceLocator.Get<ResourceService>().LoadAsset("UI_Background").ContinueWith(prefab => Instantiate(prefab)).Forget();
        }
    }
}
