using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_InGameSetting : UI_Popup
    {
        public void OnClickTitle()
        {
            GameManager.Instance.OnMoveTitle();
        }

        public void OnClickExit()
        {
            UIManager.Instance.CloseCurrentPopup();
        }
    }
}
