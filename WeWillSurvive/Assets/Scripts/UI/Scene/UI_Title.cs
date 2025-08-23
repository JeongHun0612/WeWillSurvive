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
            // ����Ƽ �����Ϳ��� ���� ���� ���� �÷��� ��带 ����
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // ����� ���ӿ����� ���ø����̼� ����
            Application.Quit();
#endif
        }
    }
}
