using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive.Log
{
    public class LogManager : IService
    {
        private List<string> _mainEventResultLogs = new();
        private List<string> _characterEventResultLogs = new();
        private List<string> _expeditionResultLogs = new();
        private List<string> _characterStatusLogs = new();

        public async UniTask InitializeAsync()
        {

            await UniTask.Yield();
        }

        public void AddMainEventResultLog(string message)
        {
            if (_mainEventResultLogs == null)
                _mainEventResultLogs = new();

            _mainEventResultLogs.Add(message);
        }

        public string GetMainEventResultLog()
        {
            if (_mainEventResultLogs == null || _mainEventResultLogs.Count == 0)
                return string.Empty;

            return string.Join("\n\n", _mainEventResultLogs);
        }
    }
}
