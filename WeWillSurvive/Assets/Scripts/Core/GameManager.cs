using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeWillSurvive.Character;
using WeWillSurvive.UI;
using WeWillSurvive.FarmingReport;
using WeWillSurvive.Log;
using WeWillSurvive.GameEvent;
using WeWillSurvive.Ending;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public int Day;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();
        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        private async void Start()
        {
            await ServiceLocator.AutoRegisterServices();
            await UIManager.Instance.InitializeAsync();
            GameEventManager.Instance.Initialize();

            if (SceneManager.GetActiveScene().name == "2D")
            {
                OnStartSurvive();
            }
        }

        public void OnStartSurvive()
        {
            Day = 0;

            CharacterManager.SettingCharacter();
            GameEventManager.Instance.ResetState();
            FarmingReportManager.Instance.UpdateFarmingReport();

            StartNextDay();
        }

        public void StartNextDay()
        {
            // EnnDayEvent 발생
            EventBus.Publish(new EndDayEvent { });

            Day++;

            if (Day > 1)
            {
                // 로그 초기화
                LogManager.ClearAllLogs();
            }

            if (!EndingManager.Instance.IsEnding)
            {
                // 이벤트 결과 반영
                GameEventManager.Instance.OnDayComplete();

                // 플레이어 상태 업데이트
                CharacterManager.UpdateCharacterStatus();

                // 이벤트 업데이트
                GameEventManager.Instance.OnNewDay();
            }

            UIManager.Instance.ShowOverlay<UI_Pade>().StartPadeSequence(OnNewDay);
        }

        private void OnNewDay()
        {
            UIManager.Instance.ClosePopups(remain: 1);

            if (UIManager.Instance.GetCurrentScene<UI_Room>() == null)
                UIManager.Instance.ShowScene<UI_Room>();

            if (EndingManager.Instance.IsEnding)
            {
                // TODO 엔딩 컷씬 출력
                return;
            }

            // 하루가 시작 시 발생하는 이벤트
            EventBus.Publish(new NewDayEvent() { CurrentDay = Day });
        }
    }
}
