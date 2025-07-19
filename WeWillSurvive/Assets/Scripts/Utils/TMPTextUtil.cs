using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

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
    }
}
