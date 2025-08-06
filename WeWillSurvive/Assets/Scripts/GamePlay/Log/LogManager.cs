using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
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
        private Dictionary<ECharacter, List<string>> _characterStatusLogs = new();

        public async UniTask InitializeAsync()
        {

            await UniTask.Yield();
        }

        public void AddMainEventResultLog(string message) => AddLogToList(_mainEventResultLogs, message);
        public void AddCharacterEventResultLog(string message) => AddLogToList(_characterEventResultLogs, message);

        public void AddExpeditionResultLog(string message) => AddLogToList(_expeditionResultLogs, message);

        public void AddExpeditionResultLog(string message, List<ExpeditionRewardItemResult> rewardItemResults = null)
        {
            if (_expeditionResultLogs == null)
                _expeditionResultLogs = new();

            string logText = message + '\n';

            if (rewardItemResults != null && rewardItemResults.Count > 0)
            {
                var rewardStrings = new List<string>();

                foreach (var rewardItemResult in rewardItemResults)
                {
                    // TMP Sprite Asset에서 EItem 이름과 동일한 Sprite name을 찾는 방식
                    rewardStrings.Add($"\n<sprite name={rewardItemResult.RewardItem}> +{rewardItemResult.Amount}");
                    //rewardStrings.Add($"[{rewardItemResult.RewardItem}] +{rewardItemResult.Amount}");
                }

                logText += string.Join(" ", rewardStrings);
            }

            _expeditionResultLogs.Add(logText);
        }

        public void AddCharacterStatusLog(ECharacter character, string message)
        {
            if (_characterStatusLogs == null)
                _characterStatusLogs = new();

            if (!_characterStatusLogs.TryGetValue(character, out var logMessages))
            {
                _characterStatusLogs[character] = new List<string> { message };
            }
            else
            {
                logMessages.Add(message);
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

            if (_characterStatusLogs.Count > 0)
            {
                foreach (var kvp in _characterStatusLogs)
                {
                    sb.AppendLine(string.Join("\n", kvp.Value));
                    sb.AppendLine();
                }
            }

            return sb.ToString().TrimEnd();
        }

        public void ClearAllLogs()
        {
            _mainEventResultLogs?.Clear();
            _characterEventResultLogs?.Clear();
            _expeditionResultLogs?.Clear();
            _characterStatusLogs?.Clear();
        }

        private void AddLogToList(List<string> target, string message)
        {
            if (target == null)
                target = new();

            target.Add(message);
        }
    }
}
