using System;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.UI;
using WeWillSurvive.FarmingReport;
using WeWillSurvive.Log;
using WeWillSurvive.GameEvent;
using WeWillSurvive.Ending;
using WeWillSurvive.Item;
using Cysharp.Threading.Tasks;
using WeWillSurvive.Expedition;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public int Day;
        public bool IsFarmingSuccess;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();
        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        [SerializeField] private GameObject FarmingPrefab;
        private GameObject FarmingObject;
        private GameObject mainCamera;

        private async void Start()
        {
            UIManager.Instance.LoadingUI.Show();

            await ServiceLocator.AutoRegisterServices();
            await UIManager.Instance.InitializeAsync();
            await SoundManager.Instance.InitializeAsync();
            await GameEventManager.Instance.InitializeAsync();

            UIManager.Instance.LoadingUI.Hide();

            OnMoveTitle();
        }

        public void OnMoveTitle()
        {
            SoundManager.Instance.PlayBGM(EBGM.BGM_Title);

            ItemManager.Dipose();
            LogManager.Dipose();
            FarmingObjectDipose();

            UIManager.Instance.CloseAllUIs();
            UIManager.Instance.ShowScene<UI_Title>();
        }

        public void OnStartParming()
        {
            UIManager.Instance.CloseAllUIs();

            ///
            mainCamera = Camera.main.gameObject;
            mainCamera.SetActive(false);
            FarmingObject = Instantiate(FarmingPrefab);
            ///

        }

        public async void OnStartSurvive()
        {
            SoundManager.Instance.PlayBGM(EBGM.BGM_Main);

            UIManager.Instance.CloseAllUIs();

            FarmingObjectDipose();

            await UIManager.Instance.ShowPopup<UI_Intro>().PlayScene();

            Day = 0;

            LogManager.ClearAllLogs();
            EndingManager.Instance.ResetState();
            ExpeditionManager.Instance.ResetState();
            BuffManager.Instance.ResetState();
            GameEventManager.Instance.ResetState();

            CharacterManager.SettingCharacter();
            FarmingReportManager.Instance.UpdateFarmingReport();

            StartNextDay();
        }

        public void StartNextDay()
        {
            Day++;

            UIManager.Instance.ShowOverlay<UI_Pade>().StartPadeSequence(OnNewDay);
        }

        public void OnEndDay()
        {
            Debug.Log("[하루 마무리] ==========================================");

            // 로그 초기화
            LogManager.ClearAllLogs();

            // 이벤트 발생
            EventBus.Publish(new EndDayEvent { });

            // 이벤트 결과 반영
            GameEventManager.Instance.OnDayComplete();

            // 새로운 날 시작
            StartNextDay();
        }

        private void OnNewDay()
        {
            Debug.Log("[하루 시작] ===========================================");

            UIManager.Instance.CloseAllPopups();

            if (UIManager.Instance.GetCurrentScene<UI_Room>() == null)
                UIManager.Instance.ShowScene<UI_Room>();

            if (UIManager.Instance.GetCurrentHUD<UI_RoomHUD>() == null)
                UIManager.Instance.ShowHUD<UI_RoomHUD>();

            // 플레이어 상태 업데이트
            CharacterManager.UpdateCharacterStatus();

            // 버프 업데이트
            BuffManager.Instance.OnNewDay();

            // 이벤트 업데이트
            GameEventManager.Instance.OnNewDay();

            // 하루가 시작 시 발생하는 이벤트
            EventBus.Publish(new NewDayEvent() { CurrentDay = Day });
        }

        private void FarmingObjectDipose()
        {
            if (mainCamera != null)
            {
                mainCamera.SetActive(true);
                mainCamera = null;
            }

            if (FarmingObject != null)
            {
                Destroy(FarmingObject);
                FarmingObject = null;
            }
        }
    }
}