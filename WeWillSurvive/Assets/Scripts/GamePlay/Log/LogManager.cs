using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
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

        private Dictionary<ECharacter, string> _characterDeadLogs = new();

        private List<ResultItemData> _ResultItemDatas = new();

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

        public void AddCharacterDeadLog(ECharacter character, string message)
        {
            _characterDeadLogs.Add(character, message);
        }

        public void AddResultItemData(ResultItemData ResultItemData)
        {
            if (_ResultItemDatas == null)
                _ResultItemDatas = new();

            _ResultItemDatas.Add(ResultItemData);
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

        public string GetCharacterDeadMessage()
        {
            if (_characterDeadLogs.Count == 0)
            {
                return string.Empty;
            }

            return string.Join("\n\n", _characterDeadLogs.Values);
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

            _ResultItemDatas?.Clear();
        }

        public void Dipose()
        {
            ClearAllLogs();
            _characterDeadLogs.Clear();
        }

        private void AddFormattedLog(List<string> logList, string message)
        {
            string logText = message;

            // 보상 아이템 데이터가 있으면 로그에 추가
            if (_ResultItemDatas != null && _ResultItemDatas.Count > 0)
            {
                logText += "\n" + GetResultItemLogText(_ResultItemDatas);
                _ResultItemDatas.Clear();
            }

            AddLogToList(logList, logText);
        }

        private void AddLogToList(List<string> target, string message)
        {
            if (target == null)
                target = new();

            target.Add(message);
        }

        private string GetResultItemLogText(List<ResultItemData> ResultItemDatas)
        {
            if (ResultItemDatas == null || !ResultItemDatas.Any())
                return string.Empty;

            var inv = CultureInfo.InvariantCulture;

            var itemLogStrings = ResultItemDatas
                .Where(entry => entry.Amount != 0) // Amount가 0이 아닌 항목만 필터링
                .Select(entry =>
                {
                    // ToString("+#;-#")는 양수일 때 '+', 음수일 때 '-'를 자동으로 붙여줍니다.
                    string formattedAmount = entry.Amount.ToString("+0.##;-0.##", inv);
                    return $"<sprite name={entry.ItemType}> {formattedAmount}";
                });

            return "\n" + string.Join(" ", itemLogStrings);
        }
    }
}