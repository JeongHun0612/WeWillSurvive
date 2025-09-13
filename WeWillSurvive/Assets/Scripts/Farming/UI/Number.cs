using UnityEngine;
using System.Collections.Generic;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class Number : MonoBehaviour
    {
        [SerializeField] private List<GameObject> segments;
        [SerializeField] private int currentNum = 0;

        private static readonly bool[,] numberPatterns =
        {  // top, top r, bottom r, low, bottom l, top l, mid
            { true,  true,  true,  true,  true,  true,  false }, // 0
            { false, true,  true,  false, false, false, false }, // 1
            { true,  true,  false, true,  true,  false, true  }, // 2
            { true,  true,  true,  true,  false, false, true  }, // 3
            { false, true,  true,  false, false, true,  true  }, // 4
            { true,  false, true,  true,  false, true,  true  }, // 5
            { true,  false, true,  true,  true,  true,  true  }, // 6
            { true,  true,  true,  false, false, false, false }, // 7
            { true,  true,  true,  true,  true,  true,  true  }, // 8
            { true,  true,  true,  true,  false, true,  true  }, // 9
        };

        public void SetDigit(int value, bool NoSound = true)
        {
            if (value < 0 || value > 9 || currentNum == value) return;
            for (int i = 0; i < segments.Count; i++)
            {
                bool shouldBeOn = numberPatterns[value, i];
                segments[i].SetActive(shouldBeOn);
            }
            currentNum = value;
            if (!NoSound) { SoundManager.Instance.PlaySFX(FarmSoundMaster.Instance.GetMusic(16)); }
        }
    }
}
