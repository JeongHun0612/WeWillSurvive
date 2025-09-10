using System;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.UI;
using WeWillSurvive.FarmingReport;
using WeWillSurvive.Log;
using WeWillSurvive.GameEvent;
using WeWillSurvive.Ending;
using WeWillSurvive.Item;

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

        private async void Start()
        {
            UIManager.Instance.LoadingUI.Show();

            await ServiceLocator.AutoRegisterServices();
            await UIManager.Instance.InitializeAsync();
            await GameEventManager.Instance.InitializeAsync();

            UIManager.Instance.LoadingUI.Hide();

            OnMoveTitle();

            IsFarmingSuccess = true;
        }

        public void OnMoveTitle()
        {
            ItemManager.Dipose();

            UIManager.Instance.CloseAllUIs();
            UIManager.Instance.ShowScene<UI_Title>();
        }

        public void OnStartParming()
        {
            UIManager.Instance.CloseAllUIs();

            // TODO 파밍 인트로 삽입
            //await UIManager.Instance.ShowPopup<UI_Intro>().PlayScene();

            //turn off current main camera
            //turn on FarmingSet - farming set will do a start sequence as it awakes and then add items as they collect them.

        }

        public async void OnStartSurvive()
        {
            UIManager.Instance.CloseAllUIs();

            // TODO 파밍씬 오브젝트 Dipose

            await UIManager.Instance.ShowPopup<UI_Intro>().PlayScene();

            Day = 0;

            LogManager.ClearAllLogs();

            CharacterManager.SettingCharacter();
            EndingManager.Instance.ResetState();
            BuffManager.Instance.ResetState();
            GameEventManager.Instance.ResetState();
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
    }
}