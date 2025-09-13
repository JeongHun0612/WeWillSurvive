using UnityEngine;
using System.Collections;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class SevenSegTimer : MonoBehaviour
    {
        public Number min1;
        public Number min2;
        public Number sec1;
        public Number sec2;
        public GameObject topDot;
        public GameObject bottomDot;
        public float starttime = 100f;
        public float lowTimeThreshold = 10f;
        public bool isFlowing = false;
        public float sirentime = 1f;

        float timeRemaining;
        bool colonVisible = true;
        bool faded = false;

        void Start()
        {
            starttime = FarmingSet.Instance.TimerTime();
            timeRemaining = starttime;
            UpdateDisplay();
        }

        public void StartSequence(float times)
        {
            sirentime = times;
            StartCoroutine(StartSiren());
        }

        private IEnumerator StartSiren()
        {
            SoundManager.Instance.PlaySFX(FarmSoundMaster.Instance.GetMusic(22));
            yield return new WaitForSeconds(sirentime);
            SoundManager.Instance.PlaySFX(FarmSoundMaster.Instance.GetMusic(22));
            yield return new WaitForSeconds(sirentime);
            SoundManager.Instance.PlaySFX(FarmSoundMaster.Instance.GetMusic(22));
            yield return new WaitForSeconds(sirentime);
            SoundManager.Instance.PlaySFX(FarmSoundMaster.Instance.GetMusic(23));
            yield return new WaitForSeconds(sirentime);
            isFlowing = true;
        }

        public void Halt()
        {
            isFlowing = false;
        }

        void Update()
        {
            if (isFlowing)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    isFlowing = false;
                    FarmingSet.Instance.EndSeq(false);
                }
                UpdateDisplay();
            }
        }

        void UpdateDisplay()
        {
            if (timeRemaining > lowTimeThreshold)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                min1.SetDigit(minutes / 10);
                min2.SetDigit(minutes % 10);
                sec1.SetDigit(seconds / 10);
                sec2.SetDigit(seconds % 10);
                bool newColonVisible = Mathf.FloorToInt(timeRemaining) % 2 == 0;
                if (colonVisible != newColonVisible)
                {
                    colonVisible = newColonVisible;
                    topDot.SetActive(colonVisible);
                    bottomDot.SetActive(colonVisible);
                }

                if (timeRemaining - lowTimeThreshold <= 5f && !faded)
                {
                    faded = true;
                    SoundManager.Instance.FadeOutBGM(5f);
                }
            }
            else
            {
                int centisec = Mathf.FloorToInt(timeRemaining * 100);
                int seconds = centisec / 100;
                int tens = seconds / 10;
                int ones = seconds % 10;
                int tenths = (centisec / 10) % 10;
                int hundredths = centisec % 10;
                min1.SetDigit(tens);
                min2.SetDigit(ones, false);
                sec1.SetDigit(tenths);
                sec2.SetDigit(hundredths);
                topDot.SetActive(false);
                bottomDot.SetActive(true);
            }
        }
    }
}
