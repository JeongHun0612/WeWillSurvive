using UnityEngine;
using System.Collections;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class StartUI : MonoBehaviour
    {
        [SerializeField] private GameObject one;
        [SerializeField] private GameObject two;

        [SerializeField] private float TotalTime = 6f;
        [SerializeField] private float fadeTime = 0.2f;
        private CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
        }

        public void StartSequence(float times)
        {
            TotalTime = times;
            StartCoroutine(Sequence(TotalTime));
        }

        private IEnumerator Sequence(float totalTime)
        {
            one.SetActive(true);
            two.SetActive(false);
            fadeTime = Mathf.Min(fadeTime, totalTime * 0.25f);
            float stayTime = (totalTime - (4f * fadeTime)) * 0.5f;
            yield return Fade(1f, fadeTime);
            yield return new WaitForSeconds(stayTime);
            yield return Fade(0f, fadeTime);
            one.SetActive(false);
            two.SetActive(true);
            yield return Fade(1f, fadeTime);
            yield return new WaitForSeconds(stayTime);
            yield return Fade(0f, fadeTime);
            one.SetActive(false);
            two.SetActive(false);
        }
        private IEnumerator Fade(float target, float duration)
        {
            float start = canvasGroup.alpha;
            float time = 0f;

            if (target == 1f) { SoundManager.Instance.PlaySFX(FarmSoundMaster.Instance.GetMusic(21)); }

            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, target, time / duration);
                yield return null;
            }

            canvasGroup.alpha = target;
        }
    }
}