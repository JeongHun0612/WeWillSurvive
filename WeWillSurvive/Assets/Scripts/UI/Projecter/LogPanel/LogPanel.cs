using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
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

        public override void RefreshPage(int startPageIndex)
        {
            StartPageIndex = startPageIndex;

            gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases();

            string logMessage = LogManager.GetMainEventResultLog();
            _pageTexts = GetPageTexts(logMessage);

            PageCount = _pageTexts.Count;
            Debug.Log("PageCount : " + PageCount);
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);

            _logText.text = _pageTexts[localIndex];
        }

        private List<string> GetPageTexts(string text)
        {
            _tempText.text = text;
            _tempText.ForceMeshUpdate();

            TMP_TextInfo textInfo = _tempText.textInfo;
            int totalLines = textInfo.lineCount;

            List<string> pages = new();
            int currentLine = 0;

            while (currentLine < totalLines)
            {
                int endLine = Mathf.Min(currentLine + _maxLineCount - 1, totalLines - 1);

                int firstVisibleChar = textInfo.lineInfo[currentLine].firstVisibleCharacterIndex;
                int lastVisibleChar = textInfo.lineInfo[endLine].lastVisibleCharacterIndex;

                if (firstVisibleChar < 0 || lastVisibleChar < 0 || lastVisibleChar < firstVisibleChar)
                {
                    currentLine += _maxLineCount;
                    continue;
                }

                StringBuilder builder = new();

                for (int i = firstVisibleChar; i <= lastVisibleChar; i++)
                {
                    var charInfo = textInfo.characterInfo[i];
                    char c = charInfo.character;

                    if (c >= 0xE000 && c <= 0xF8FF)
                    {
                        int spriteIndex = c - 0xE000;
                        builder.Append($"<sprite={spriteIndex}>");
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }

                string pageText = builder.ToString();
                pages.Add(pageText);
                currentLine += _maxLineCount;
            }

            _tempText.text = string.Empty;
            return pages;
        }
    }
}
