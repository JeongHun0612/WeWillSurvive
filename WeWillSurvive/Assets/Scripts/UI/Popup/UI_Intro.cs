using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_Intro : UI_Popup
    {
        [Header("##Cut Scene")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image[] _cutScenes;

        [Header("##Shake Value")]
        [SerializeField] private Vector3 _shakeStrength;
        [SerializeField] private float _shakeDruation;

        [Header("## CutScene Tween")]
        [Header("## PadeIn")]
        [SerializeField] private float _padeInDelay = 0.5f;
        [SerializeField] private float _padeInDuration = 1f;
        [SerializeField] private Ease _padeInEase = Ease.OutCubic;

        [Header("## PadeOut")]
        [SerializeField] private float _padeOutDuration = 1f;
        [SerializeField] private Ease _padeOutEase = Ease.InCubic;
        [SerializeField] private float _skipFadeOutDuration = 0.5f;

        [Header("## Scene")]
        [SerializeField] private float _sceneDelay = 2f;

        private bool _isSkip;
        private Sequence _currentSeq;

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                if (_isSkip) return;

                _isSkip = true;

                // 진행 중인 시퀀스를 즉시 중단
                if (_currentSeq != null && _currentSeq.IsActive())
                    _currentSeq.Kill(false);
            }
        }

        public async UniTask PlayScene()
        {
            _isSkip = false;

            foreach (var scene in _cutScenes)
            {
                if (scene != null)
                    scene.gameObject.SetActive(false);
            }
            _canvasGroup.alpha = 0f;

            for (int i = 0; i < _cutScenes.Length; i++)
            {
                if (_isSkip) break;

                var scene = _cutScenes[i];
                scene.gameObject.SetActive(true);

                // Shake
                //if (i == 0)
                //{
                //    _cutScenes[i].transform.DOShakePosition(
                //        duration: _shakeDruation,
                //        strength: _shakeStrength,
                //        vibrato: 10,
                //        randomness: 90,
                //        snapping: true,
                //        fadeOut: true
                //        );
                //}

                // 페이드인 + 유지시간(딜레이)을 하나의 시퀀스로 구성
                _currentSeq = DOTween.Sequence()
                    .AppendInterval(_padeInDelay)
                    .Append(_canvasGroup.DOFade(1f, _padeInDuration)
                    .SetEase(_padeInEase))
                    .AppendInterval(_sceneDelay);


                // 시퀀스가 끝나거나(Kill 포함) 하면 즉시 다음 줄로 진행
                await _currentSeq.AsyncWaitForCompletion();

                if (_isSkip)
                {
                    // 현재 알파에서 0까지
                    await _canvasGroup
                        .DOFade(0f, _skipFadeOutDuration)
                        .SetEase(_padeOutEase)
                        .AsyncWaitForCompletion();
                    scene.gameObject.SetActive(false);
                    break;
                }

                // 일반 흐름: 페이드아웃
                await _canvasGroup
                    .DOFade(0f, _padeOutDuration)
                    .SetEase(_padeOutEase)
                    .AsyncWaitForCompletion();
                scene.gameObject.SetActive(false);

                //// PadeIn
                //float elapsed = 0f;
                //while (elapsed < _padeInDuration)
                //{
                //    if (_isSkip) break;

                //    elapsed += Time.deltaTime;
                //    //float alpha = Mathf.Clamp01(elapsed / _padeInDuration);
                //    //_canvasGroup.alpha = alpha;

                //    Debug.Log($"{elapsed} {elapsed / _padeInDuration}");

                //    _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / _padeInDuration);

                //    await UniTask.Yield();
                //}

                //// Delay
                //float waitTime = 0f;
                //while (waitTime < _sceneDelay)
                //{
                //    if (_isSkip) break;

                //    waitTime += Time.deltaTime;
                //    await UniTask.Yield();
                //}

                //// PadeOut
                //float startAlpha = _canvasGroup.alpha;
                //elapsed = 0f;
                //while (elapsed < _padeOutDuration)
                //{
                //    elapsed += Time.deltaTime;
                //    float alpha = Mathf.Clamp01(elapsed / _padeOutDuration);
                //    _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, alpha);

                //    await UniTask.Yield();
                //}

                //_cutScenes[i].gameObject.SetActive(false);
            }

            _currentSeq = null;
            UIManager.Instance.CloseCurrentPopup();
        }
    }
}