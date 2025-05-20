using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using static Define;

namespace WeWillSurvive
{
    public class UI_Main : UI_Popup
    {
        [SerializeField] Button _leadButton;
        [SerializeField] Button _cookButton;
        [SerializeField] Button _bellButton;
        [SerializeField] Button _drKButton;

        protected override void Init()
        {
            base.Init();

            UI_Background ui = null;
            if (GameManager.Instance.SceneUI is UI_Background)
            {
                ui = GameManager.Instance.SceneUI as UI_Background;
                _leadButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.Lead));
                _cookButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.Cook));
                _bellButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.Bell));
                _drKButton .onClick.AddListener(() => ui.ChangeBackground(ERoom.DrK));
            }
        }
    }
}