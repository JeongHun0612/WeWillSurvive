using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_Black : UI_Overlay
    {
        [SerializeField] GameObject _black;
        [SerializeField] float _wipeDuration = 0.5f;
        [SerializeField] float _fadeDuration = 1f;

        // TEMP
        const float SCREEN_WIDTH = 1920f;

        /// <param name="right"> 왼쪽에서 오른쪽으로 이동 </param>
        /// <param name="coverAction"> 화면 덮은 후에 할 행동 </param>
        public void Wipe(bool right, Action coverAction = null, Action finishAction = null)
        {
            StartCoroutine(CoWipe(right, coverAction, finishAction));
        }

        public void FadeIO(Action coverAction = null, Action finishAction = null)
        {
            StartCoroutine(CoFadeIO(coverAction, finishAction));
        }

        private void Init()
        {
            _black.transform.localPosition = new Vector3(-(SCREEN_WIDTH + 50f), 0f, 0f);
            _black.GetComponent<Image>().color = Color.black;
            UIManager.Instance.CloseCurrentOverlay();
        }

        private IEnumerator CoWipe(bool right, Action coverAction, Action finishAction)
        {
            _black.transform.DOKill();
            int sign = right ? 1 : -1;
            _black.transform.localPosition = new Vector3(-(SCREEN_WIDTH + 50f) * sign, 0f, 0f);

            yield return _black.transform.DOLocalMoveX(0f, _wipeDuration).WaitForCompletion();

            coverAction?.Invoke();

            yield return _black.transform.DOLocalMoveX((SCREEN_WIDTH + 50f) * sign, _wipeDuration).WaitForCompletion();

            finishAction?.Invoke();

            Init();
        }

        private IEnumerator CoFadeIO(Action coverAction, Action finishAction)
        {
            _black.transform.DOKill();

            _black.transform.localPosition = Vector3.zero;
            Image image = _black.GetComponent<Image>();
            image.color = new Color(0, 0, 0, 0);

            yield return image.DOFade(1.0f, _fadeDuration).WaitForCompletion();

            coverAction?.Invoke();

            yield return image.DOFade(0.0f, _fadeDuration).WaitForCompletion();

            finishAction?.Invoke();

            Init();
        }
    }
}
