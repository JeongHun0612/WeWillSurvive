using UnityEngine;
using System.Collections;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class FarmingSet : MonoBehaviour
    {
        public static FarmingSet Instance { get; private set; }
        [SerializeField] private bool debugTest = false;
        [SerializeField] private float timerTime = 200f;
        [SerializeField] private UICanvas uiCanvas;
        [SerializeField] private OrbitPlayer player;
        [SerializeField] private Blackout blackout;
        [SerializeField] private float bias = 1f;
        [SerializeField] private float startui = 6f;
        [SerializeField] private float slide = 2f;
        [SerializeField] private float countdown = 1f;
        [SerializeField] private float endslide = 0.3f;
        [SerializeField] private float blackTime = 0.5f;
        [SerializeField] private float redTime = 3f;
        [SerializeField] private float textTime = 1f;
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            StartCoroutine(StartSeq());
        }

        public float TimerTime()
        {
            return timerTime;
        }

        private IEnumerator StartSeq()
        {
            yield return new WaitForSeconds(1f);
            CameraEffects.Instance.NudgeLeft(slide, bias + startui);
            yield return new WaitForSeconds(slide);
            uiCanvas.StartSequence(bias, startui, countdown, slide);
            yield return new WaitForSeconds(bias + startui + (countdown * 4f) + slide);
            player.StartPlayer();
        }

        public void EndSeq(bool success)
        {
            player.EndPlayer();
            if (success)
            {
                Debug.Log("Success Ending");
                StartCoroutine(SuccessSeq());
            }
            else
            {
                Debug.Log("Failed Ending");
                StartCoroutine(FailSeq());
            }
        }

        private IEnumerator SuccessSeq()
        {
            uiCanvas.EndSequence(bias, endslide);
            yield return new WaitForSeconds(bias + endslide);
            blackout.BlackOut(blackTime);
            yield return new WaitForSeconds(blackTime);
            BacktoYou(true);
        }

        private IEnumerator FailSeq()
        {
            uiCanvas.EndSequence(bias, endslide);
            yield return new WaitForSeconds(bias + endslide);
            blackout.FailOut(redTime, textTime);
            yield return new WaitForSeconds(redTime + textTime + 2f);
            BacktoYou(false);
        }

        private void BacktoYou(bool success)
        {
            if (debugTest) { Debug.Log("The game has ended"); }
            else
            {
                //if(!success){}
                GameManager.Instance.OnStartSurvive();
            }
        }
    }
}
