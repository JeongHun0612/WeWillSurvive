using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_InGameSetting : UI_Popup
    {
        public override void OnHide()
        {
            Time.timeScale = 1f;
        }

        public void OnClickTitle()
        {
            SoundManager.Instance.PlaySFX(ESFX.SFX_Click_2);

            GameManager.Instance.OnMoveTitle();
        }

        public void OnClickExit()
        {
            SoundManager.Instance.PlaySFX(ESFX.SFX_Click_2);

            UIManager.Instance.CloseCurrentPopup();
        }
    }
}
