using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using static Define;

namespace WeWillSurvive
{
    public class UI_Main : UI_Popup
    {
        [SerializeField] Button _roomMonitorButton;

        UI_Background ui;

        protected override void Init()
        {
            base.Init();

            ui = null;
            if (GameManager.Instance.SceneUI is not UI_Background)
            {
                Debug.LogError("[UI_Main] 2D Scene���� ������ �ʾ���");
                return;
            }
            ui = GameManager.Instance.SceneUI as UI_Background;

            // 
            _roomMonitorButton.onClick.AddListener(() =>
                ServiceLocator.Get<ResourceService>().LoadAsset("UI_RoomMonitor").ContinueWith(prefab => Instantiate(prefab)).Forget());

            // TODO: ������ ��ġ
            float cnt = GameManager.Instance.GetItemCount(EItem.Water);
            // �� ���� ��ŭ ��ġ
        }
    }
}