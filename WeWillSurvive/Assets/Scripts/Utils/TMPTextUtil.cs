using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeWillSurvive
{
    public class TMPTextUtil : MonoBehaviour
    {
        public static List<string> SplitTextByLines(TMP_Text tmpText, string text, int maxLineCount)
        {
            tmpText.text = text;
            tmpText.ForceMeshUpdate();

            TMP_TextInfo textInfo = tmpText.textInfo;
            int totalLines = textInfo.lineCount;

            List<string> pages = new();
            int currentLine = 0;

            while (currentLine < totalLines)
            {
                int endLine = Mathf.Min(currentLine + maxLineCount - 1, totalLines - 1);

                if (currentLine >= textInfo.lineInfo.Length || endLine >= textInfo.lineInfo.Length)
                {
                    Debug.LogWarning("SplitTextByLines: lineInfo 인덱스 초과 감지됨.");
                    break;
                }

                int firstVisibleChar = textInfo.lineInfo[currentLine].firstVisibleCharacterIndex;
                int lastVisibleChar = textInfo.lineInfo[endLine].lastVisibleCharacterIndex;

                if (firstVisibleChar < 0 || lastVisibleChar < 0 || lastVisibleChar < firstVisibleChar)
                {
                    currentLine += maxLineCount;
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

                pages.Add(builder.ToString());
                currentLine += maxLineCount;
            }

            tmpText.text = string.Empty;
            return pages;
        }

        public static int CalculateMaxLineCount(TMP_Text tmpText)
        {
            // 텍스트 영역의 전체 높이
            float totalHeight = ((RectTransform)tmpText.transform).rect.height;

            // 실제 한 줄 높이 추정 (기본 폰트 크기 + 줄 간격 보정)
            float lineHeight = tmpText.fontSize * (1 + tmpText.lineSpacing);

            // 줄 수 = 전체 높이 / 줄 높이
            return Mathf.FloorToInt(totalHeight / lineHeight);
        }
    }
}
