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
            FarmingReportManager.Instance.UpdateFarmingReport();

            StartNextDay();
        }

        public void StartNextDay()
        {
            UIManager.Instance.ShowOverlay<UI_Pade>().StartPadeSequence(OnNewDay);
        }

        private void OnNewDay()
        {
            Day++;

            UIManager.Instance.ClosePopups(remain: 1);

            if (UIManager.Instance.GetCurrentScene<UI_Room>() == null)
                UIManager.Instance.ShowScene<UI_Room>();

            CharacterManager.UpdateCharacterStatus();

            // 모든 플레이어가 사망 시 생존 실패
            if (CharacterManager.AliveCharacterCount() == 0)
            {
                Debug.Log("Game End");
                return;
            }

            // TOOD 엔딩 분기 확인
            EventBus.Publish(new NewDayEvent() { CurrentDay = Day });

            // TODO 로그 초기화
            LogManager.ClearAllLogs();
        }
    }
}
