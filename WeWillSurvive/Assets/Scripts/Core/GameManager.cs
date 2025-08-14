using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeWillSurvive.Character;
using WeWillSurvive.Status;
using WeWillSurvive.UI;
using WeWillSurvive.Item;
using WeWillSurvive.FarmingReport;
using WeWillSurvive.Log;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Ending;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public int Day;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();
        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        private async void Start()
        {
            await ServiceLocator.AutoRegisterServices();
            await UIManager.Instance.InitializeAsync();

            if (SceneManager.GetActiveScene().name == "2D")
            {
                OnStartSurvive();
            }
        }

        public void OnStartSurvive()
        {
            Day = 0;

            CharacterManager.SettingCharacter();

            MainEventManager.Instance.ResetState();
            EndingManager.Instance.ResetState();

            FarmingReportManager.Instance.UpdateFarmingReport();

            StartNextDay();
        }

        public void StartNextDay()
        {
            if (!EndingManager.Instance.IsEnding)
            {
                // 플레이어 상태 업데이트
                CharacterManager.UpdateCharacterStatus();

                // 이벤트 매니져 업데이트
                MainEventManager.Instance.OnNewDay();
                EndingManager.Instance.OnNewDay();
            }

            UIManager.Instance.ShowOverlay<UI_Pade>().StartPadeSequence(OnNewDay);
        }

        private void OnNewDay()
        {
            Day++;

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

            // 로그 초기화
            LogManager.ClearAllLogs();
        }
    }
}
