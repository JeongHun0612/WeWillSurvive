using UnityEngine;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private void Start()
        {
			ServiceLocator.AutoRegisterServices();
        }
    }
}
