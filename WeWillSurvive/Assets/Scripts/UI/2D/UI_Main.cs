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
                Debug.LogError("[UI_Main] 2D Scene에서 열리지 않았음");
                return;
            }
            ui = GameManager.Instance.SceneUI as UI_Background;

            // Room Monitor
            _roomMonitorButton.onClick.AddListener(() =>
                ServiceLocator.Get<ResourceService>().LoadAsset("UI_RoomMonitor").ContinueWith(prefab => Instantiate(prefab)).Forget());

            // 우주 기지 내 존재하지 않으면 캐릭터 비활성화
            CharacterInfo[] infos = CharacterManager.Instance.CharacterInfos;
            foreach (CharacterInfo info in infos)
            {
                if (info.Status == ECharacterStatus.None)
                {
                    Transform t = transform.Find($"Characters/{info.Name}");
                    if (t != null)
                        t.gameObject.SetActive(false);
                }
            }

            // TODO: 아이템 배치
            float cnt = GameManager.Instance.GetItemCount(EItem.Water);
            // 물 개수 만큼 배치
        }
    }
}