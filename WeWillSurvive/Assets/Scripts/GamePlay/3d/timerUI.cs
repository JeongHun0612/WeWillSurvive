using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

namespace WeWillSurvive
{
    public class timerUI : MonoBehaviour
    {
        public float StartTime;
        public float EmergencyTime;
        private float timeleft;

        public TMP_Text timer1;
        public TMP_Text timer2;
        public bool timerActive = false;
        private void Start()
        {
            timer1.gameObject.SetActive(false);
            timer2.gameObject.SetActive(false);
            ActivateTimer(); //test code. erase and replace later
        }
        private void Update()
        {
            if (timerActive)
            {
                timeleft -= Time.deltaTime;
                if (timeleft > EmergencyTime)
                {
                    float mins = Mathf.FloorToInt(timeleft / 60);
                    float secs = Mathf.FloorToInt(timeleft % 60);
                    timer1.text = string.Format("{0:00}:{1:00}", mins, secs);
                }
                else if (timeleft > 0)
                {
                    timer1.gameObject.SetActive(false);
                    timer2.gameObject.SetActive(true);
                    timer2.text = timeleft.ToString("0.00");
                }
                else
                {
                    timerActive = false;
                    Debug.Log("Timer has ended. You died.");
                    EndGame(false);
                }
            }

        }

        public void ActivateTimer()
        {
            timer1.gameObject.SetActive(true);
            timer2.gameObject.SetActive(false);
            timeleft = StartTime;
            timerActive = true;
        }

        public void EndGame(bool success = true)
        {
            if (success)
            {
                Debug.Log("Escaped before timer. You liveds.");
            }
            else
            {
                Debug.Log("Timer has ended. You died.");
            }
        }
    }
}
