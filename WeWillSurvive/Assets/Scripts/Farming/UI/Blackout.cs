using UnityEngine;
using System.Collections;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class Blackout : MonoBehaviour
    {
        public CanvasGroup Red;
        public CanvasGroup Black;
        public GameObject texts;

        public void TurnOut(float turnOut)
        {
            StartCoroutine(BlackFade(turnOut, false));
        }

        public void BlackOut(float blackOut)
        {
            StartCoroutine(BlackFade(blackOut));
        }

        public void FailOut(float redOut, float textTime)
        {
            StartCoroutine(FailFade(redOut, textTime));
        }

        private IEnumerator BlackFade(float blackOut, bool dark = true)
        {
            float duration = blackOut;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                Black.alpha = dark ? Mathf.Lerp(0f, 1f, time / duration) : Mathf.Lerp(1f, 0f, time / duration);
                yield return null;
            }
        }

        private IEnumerator FailFade(float redOut, float textTime)
        {
            float duration = redOut;
            float time = 0f;

            SoundManager.Instance.PlaySFX(FarmSoundMaster.Instance.GetMusic(33), duration);
            CameraEffects.Instance.ZoomOutShake(duration);
            while (time < duration)
            {
                time += Time.deltaTime;
                Red.alpha = Mathf.Lerp(0f, 1f, time / duration);
                yield return null;
            }

            SoundManager.Instance.StopAllSFX();
            Black.alpha = 1f;
            yield return new WaitForSeconds(1f);

            SoundManager.Instance.PlaySFX(FarmSoundMaster.Instance.GetMusic(34));
            texts.SetActive(true);
            yield return new WaitForSeconds(textTime);
            texts.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
    }
}
