using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Log;

namespace WeWillSurvive
{
    public class LogPanel : PagePanel
    {
        [SerializeField] private TMP_Text _logText;
        [SerializeField] private TMP_Text _tempText;

        [SerializeField] private int _maxLineCount;

        private List<string> _pageTexts = new();

        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public override void Initialize()
        {
            PanelType = EPanelType.Log;
        }

        public override async UniTask RefreshPageAsync(int startPageIndex)
        {
            await base.RefreshPageAsync(startPageIndex);

            string logMessage = LogManager.GetLogMessage();

            gameObject.SetActive(true);
            await UniTask.NextFrame();

            _pageTexts = TMPTextUtil.SplitTextByLines(_tempText, logMessage, _maxLineCount);
            PageCount = _pageTexts.Count;
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);

            _logText.text = _pageTexts[localIndex];
        }
    }
}
