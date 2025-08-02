using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.Log;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive.FarmingReport
{
    public enum EParmingReportType
    {
        ReturnCrew,     // 생존자 수
        FoodSupply,     // 식량 수
        WaterSupply,    // 물 수
        SupportsSupply  // 기타 물자 수
    }

    public class FarmingReportManager : MonoSceneSingleton<FarmingReportManager>
    {
        [SerializeField] private List<FarmingReportSO> _farmingReportList;

        private Dictionary<EParmingReportType, List<FarmingReportSO>> _farmingReportDicts = new();

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        protected override void Awake()
        {
            base.Awake();

            InitializeFarmingReports();
        }

        public void UpdateFarmingReport()
        {
            // 크루 인원에 따른 결과 텍스트
            int crewCount = CharacterManager.AliveCharacterCount();
            LogReportByValue(EParmingReportType.ReturnCrew, crewCount);

            // 식량에 따른 결과 텍스트
            int foodCount = (int)ItemManager.GetItemCount(EItem.Food);
            LogReportByValue(EParmingReportType.FoodSupply, foodCount);

            // 물에 따른 결과 텍스트
            int waterCount = (int)ItemManager.GetItemCount(EItem.Water);
            LogReportByValue(EParmingReportType.WaterSupply, waterCount);

            // 다른 도구에 따른 결과 텍스트
            int supportsCount = (int)ItemManager.GetSupportItemsCount();
            LogReportByValue(EParmingReportType.SupportsSupply, supportsCount);
        }

        private void InitializeFarmingReports()
        {
            if (_farmingReportDicts == null)
                _farmingReportDicts = new();

            _farmingReportDicts.Clear();

            foreach (EParmingReportType type in System.Enum.GetValues(typeof(EParmingReportType)))
            {
                _farmingReportDicts[type] = new();
            }

            foreach (var report in _farmingReportList)
            {
                _farmingReportDicts[report.Type].Add(report);
            }
        }

        private void LogReportByValue(EParmingReportType type, int value)
        {
            var reports = GetFarmingReportList(type);
            if (reports == null) return;

            foreach (var report in reports)
            {
                if (report.IsMatch(value))
                {
                    LogManager.AddMainEventResultLog(report.ReportText);
                    break;
                }
            }
        }

        private List<FarmingReportSO> GetFarmingReportList(EParmingReportType type)
        {
            if (_farmingReportDicts.TryGetValue(type, out var parmingReportList))
            {
                return parmingReportList;
            }

            return null;
        }
    }
}
