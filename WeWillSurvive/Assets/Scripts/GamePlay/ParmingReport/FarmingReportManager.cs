using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.Log;

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
        [SerializeField] private List<FarmingReportData> _farmingReportDatas;

        [SerializeField]
        [TextArea(3, 5)]
        private string _farmingFailReportText;

        private Dictionary<EParmingReportType, FarmingReportData> _farmingReportDicts = new();

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        protected override void Awake()
        {
            base.Awake();

            if (_farmingReportDicts == null)
                _farmingReportDicts = new();

            _farmingReportDicts.Clear();
            foreach (var data in _farmingReportDatas)
            {
                if (!_farmingReportDicts.ContainsKey(data.Type))
                {
                    _farmingReportDicts.Add(data.Type, data);
                }
            }
        }

        public void UpdateFarmingReport()
        {
            // 크루 인원에 따른 결과 텍스트
            int crewCount = CharacterManager.AliveCharactersCount();
            LogReportByValue(EParmingReportType.ReturnCrew, crewCount);

            // 파밍 후 탈출 성공 여부 텍스트
            if (!GameManager.Instance.IsFarmingSuccess)
                LogManager.AddMainEventResultLog(_farmingFailReportText);

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

        private void LogReportByValue(EParmingReportType type, int value)
        {
            var data = GetFarmingReportData(type);
            var reportText = data.GetFarmingReportText(value);
            LogManager.AddMainEventResultLog(reportText);
        }

        private FarmingReportData GetFarmingReportData(EParmingReportType type)
        {
            if (_farmingReportDicts.TryGetValue(type, out var farmingReportData))
            {
                return farmingReportData;
            }

            return null;
        }
    }
}
