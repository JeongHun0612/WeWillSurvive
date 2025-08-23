using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_Title : UI_Scene
    {
        public void OnClickGameStart()
        {
            GameManager.Instance.OnStartSurvive();
        }

        public void OnClickGameQuit()
        {
#if UNITY_EDITOR
            // 유니티 에디터에서 실행 중일 때는 플레이 모드를 종료
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // 빌드된 게임에서는 어플리케이션 종료
            Application.Quit();
#endif
        }
    }
}
