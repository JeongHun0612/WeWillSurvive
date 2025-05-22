using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using static Define;

namespace WeWillSurvive
{
    public class UI_RoomMonitor : UI_Popup
    {
        [SerializeField] Button _leadButton;
        [SerializeField] Button _cookButton;
        [SerializeField] Button _bellButton;
        [SerializeField] Button _drKButton;

        UI_Background ui;

        protected override void Init()
        {
            base.Init();

            ui = null;
            if (GameManager.Instance.SceneUI is not UI_Background)
            {
                Debug.LogError("[UI_RoomMonitor] 2D Scene에서 열리지 않았음");
                return;
            }
            ui = GameManager.Instance.SceneUI as UI_Background;

            _leadButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.Lead));
            _cookButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.Cook));
            _bellButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.Bell));
            _drKButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.DrK));
        }
    }
}