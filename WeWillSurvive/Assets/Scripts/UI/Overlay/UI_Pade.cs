using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Ending;

namespace WeWillSurvive.UI
{
    public class UI_Pade : UI_Overlay
    {
        [SerializeField] private Image _padeImage;
        [SerializeField] private TMP_Text _dayText;

        public void StartPadeSequence(Action callback = null)
        {
            _padeImage.color = Color.clear;
            _dayText.alpha = 0f;

            Sequence fadeSequence = DOTween.Sequence();

            fadeSequence
                .Append(_padeImage.DOFade(1f, 1f).SetEase(Ease.InCubic))
                .AppendCallback(() =>
                {
                    callback?.Invoke();
                    _dayText.text = (EndingManager.Instance.IsEnding) ? "게임 종료" : $"Day {GameManager.Instance.Day}";
                    _dayText.DOFade(1f, 1f).SetEase(Ease.InCubic);
                })
                .AppendInterval(2f)
                .Append(_padeImage.DOFade(0f, 1f).SetEase(Ease.OutCubic))
                .Join(_dayText.DOFade(0f, 1f).SetEase(Ease.OutCubic))
                .OnComplete(() => UIManager.Instance.CloseCurrentOverlay());
        }
    }
}
