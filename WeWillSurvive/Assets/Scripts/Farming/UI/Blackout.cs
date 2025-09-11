using UnityEngine;
using System.Collections;

namespace WeWillSurvive
{
    public class Blackout : MonoBehaviour
    {
        public CanvasGroup Red;
        public CanvasGroup Black;
        public GameObject texts;
        public void BlackOut(float blackOut)
        {
            StartCoroutine(BlackFade(blackOut));
        }

        public void FailOut(float redOut, float textTime)
        {
            StartCoroutine(FailFade(redOut, textTime));
        }

        private IEnumerator BlackFade(float blackOut)
        {
            float duration = blackOut;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                Black.alpha = Mathf.Lerp(0f, 1f, time / duration);
                yield return null;
            }
        }

        private IEnumerator FailFade(float redOut, float textTime)
        {
            float duration = redOut;
            float time = 0f;

            CameraEffects.Instance.ZoomOutShake(duration);
            while (time < duration)
            {
                time += Time.deltaTime;
                Red.alpha = Mathf.Lerp(0f, 1f, time / duration);
                yield return null;
            }


            Black.alpha = 1f;
            yield return new WaitForSeconds(1f);

            texts.SetActive(true);
            yield return new WaitForSeconds(textTime);
            texts.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
    }
}
