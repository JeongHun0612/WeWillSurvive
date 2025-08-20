using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;

namespace WeWillSurvive.Log
{
    public class LogManager : IService
    {
        private List<string> _mainEventResultLogs = new();
        private List<string> _characterEventResultLogs = new();
        private List<string> _expeditionResultLogs = new();
        private Dictionary<ECharacter, List<string>> _characterStatusLogs = new()
        {
            [ECharacter.Lead] = new(),
            [ECharacter.Cook] = new(),
            [ECharacter.Bell] = new(),
            [ECharacter.DrK] = new(),
        };

        private List<RewardItemData> _rewardItemDatas = new();

        public async UniTask InitializeAsync()
        {
            await UniTask.Yield();
        }

        public void AddCharacterEventResultLog(string message) => AddFormattedLog(_characterEventResultLogs, message);
        public void AddMainEventResultLog(string message) => AddFormattedLog(_mainEventResultLogs, message);
        public void AddExpeditionResultLog(string message) => AddFormattedLog(_expeditionResultLogs, message);
        public void AddCharacterStatusLog(ECharacter character, string message)
        {
            if (_characterStatusLogs.TryGetValue(character, out var logMessages))
            {
                logMessages.Add(message);
            }
            else
            {
                Debug.LogWarning($"[AddCharacterStatusLog] : {character} 타입의 로그 리스트가 존재하지 않습니다.");
            }
        }

        public void AddRewardItemData(RewardItemData rewardItemData)
        {
            if (_rewardItemDatas == null)
                _rewardItemDatas = new();

            _rewardItemDatas.Add(rewardItemData);
        }

        public void ClearCharacterStatusLog(ECharacter character)
        {
            if (_characterStatusLogs.ContainsKey(character))
            {
                _characterStatusLogs[character].Clear();
            }
        }

        public string GetLogMessage()
        {
            StringBuilder sb = new();

            void AppendSection(List<string> logs)
            {
                if (logs.Count == 0) return;

                sb.AppendLine(string.Join("\n\n", logs));
                sb.AppendLine();
            }

            AppendSection(_mainEventResultLogs);
            AppendSection(_characterEventResultLogs);
            AppendSection(_expeditionResultLogs);

            foreach (var statusLog in _characterStatusLogs.Values)
            {
                if (statusLog == null || statusLog.Count <= 0)
                    continue;

                sb.AppendLine(string.Join("\n", statusLog));
                sb.AppendLine();
            }

            return sb.ToString().TrimEnd();
        }

        public void ClearAllLogs()
        {
            _mainEventResultLogs?.Clear();
            _characterEventResultLogs?.Clear();
            _expeditionResultLogs?.Clear();

            foreach (var statusLog in _characterStatusLogs.Values)
            {
                statusLog?.Clear();
            }

            _rewardItemDatas?.Clear();
        }

        private void AddFormattedLog(List<string> logList, string message)
        {
            string logText = message;

            // 보상 아이템 데이터가 있으면 로그에 추가
            if (_rewardItemDatas != null && _rewardItemDatas.Count > 0)
            {
                logText += "\n" + GetRewardItemLogText(_rewardItemDatas);
                _rewardItemDatas.Clear();
            }

            AddLogToList(logList, logText);
        }

        private void AddLogToList(List<string> target, string message)
        {
            if (target == null)
                target = new();

            target.Add(message);
        }

        private string GetRewardItemLogText(List<RewardItemData> rewardItemDatas)
        {
            if (rewardItemDatas == null || !rewardItemDatas.Any())
                return string.Empty;

            var itemLogStrings = rewardItemDatas
                .Where(entry => entry.Amount != 0) // Amount가 0이 아닌 항목만 필터링
                .Select(entry =>
                {
                    // ToString("+#;-#")는 양수일 때 '+', 음수일 때 '-'를 자동으로 붙여줍니다.
                    string formattedAmount = entry.Amount.ToString("+#;-#");
                    return $"<sprite name={entry.ItemType}> {formattedAmount}";
                });

            return "\n" + string.Join(" ", itemLogStrings);
        }
    }
}