using UnityEngine;
using System.Collections;

namespace WeWillSurvive
{
    public class StartUI : MonoBehaviour
    {
        [SerializeField] private GameObject move;
        [SerializeField] private GameObject collect;
        [SerializeField] private GameObject returns;

        [SerializeField] private float fadeTime = 0.2f;
        private CanvasGroup canvasGroup;

        void Awake()
        {
            move.SetActive(true);
            collect.SetActive(true);
            returns.SetActive(false);

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            StartSequence(6f);
            move.SetActive(false);
            collect.SetActive(false);
            returns.SetActive(false);
        }

        public void StartSequence(float totalTime)
        {
            StartCoroutine(Sequence(totalTime));
        }

        private IEnumerator Sequence(float totalTime)
        {
            fadeTime = Mathf.Min(fadeTime, totalTime * 0.25f);
            float stayTime = (totalTime - (4f * fadeTime)) * 0.5f;
            yield return Fade(1f, fadeTime);
            yield return new WaitForSeconds(stayTime);
            yield return Fade(0f, fadeTime);
            move.SetActive(false);
            collect.SetActive(false);
            returns.SetActive(true);
            yield return Fade(1f, fadeTime);
            yield return new WaitForSeconds(stayTime);
            yield return Fade(0f, fadeTime);
        }
        private IEnumerator Fade(float target, float duration)
        {
            float start = canvasGroup.alpha;
            float time = 0f;

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