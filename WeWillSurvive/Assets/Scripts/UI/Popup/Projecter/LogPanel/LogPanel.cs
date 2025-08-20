using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Log;

namespace WeWillSurvive
{
    public class LogPanel : PagePanel
    {
        [SerializeField] private TMP_Text _logText;

        private List<string> _pageTexts = new();
        private int _maxLineCount;

        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public async override UniTask InitializeAsync()
        {
            PanelType = EPanelType.Log;

            _maxLineCount = TMPTextUtil.CalculateMaxLineCount(_logText);

            await UniTask.CompletedTask;
        }

        public override async UniTask RefreshPageAsync(int startPageIndex)
        {
            await base.RefreshPageAsync(startPageIndex);

            string logMessage = LogManager.GetLogMessage();

            gameObject.SetActive(true);
            await UniTask.NextFrame();

            _pageTexts = TMPTextUtil.SplitTextByLines(_logText, logMessage, _maxLineCount);
            PageCount = _pageTexts.Count;
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);

            _logText.text = _pageTexts[localIndex];
        }
    }
}
