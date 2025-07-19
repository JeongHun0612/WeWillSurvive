using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

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
