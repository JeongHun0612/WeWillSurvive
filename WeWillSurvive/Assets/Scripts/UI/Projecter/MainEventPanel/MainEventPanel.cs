using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Log;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive
{
    public class MainEventPanel : PagePanel
    {
        [Header("## Event Text")]
        [SerializeField] private RectTransform _textRootLayout;
        [SerializeField] private TMP_Text _eventText;

        [Header("## Choice Objects")]
        [SerializeField] private List<ChoiceOption> _choiceOptions;

        private List<string> _pageTexts = new();

        private MainEventData _mainEventData = null;
        private ChoiceOption _selectedOption = null;

        private int _maxLineCount;

        public Action ChoiceImageSelected;


        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public override void Initialize()
        {
            PanelType = EPanelType.MainEvent;

            foreach (var choiceImage in _choiceOptions)
            {
                choiceImage.Initialize(OnClickChoiceImage);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_textRootLayout);
            _maxLineCount = TMPTextUtil.CalculateMaxLineCount(_eventText);
        }

        public override async UniTask RefreshPageAsync(int startPageIndex)
        {
            await base.RefreshPageAsync(startPageIndex);

            _mainEventData = MainEventManager.Instance.GetDailyMainEvent();
            _selectedOption = null;

            if (_mainEventData == null)
            {
                PageCount = 0;
                return;
            }

            // 선택지 업데이트
            UpdateChoiceOptions(_mainEventData);

            gameObject.SetActive(true);
            await UniTask.NextFrame();

            _pageTexts = TMPTextUtil.SplitTextByLines(_eventText, _mainEventData.descriptions[0], _maxLineCount);
            PageCount = _pageTexts.Count;
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);

            if (localIndex == PageCount - 1)
                AllChoiceOptionEnabled();
            else
                AllChoiceOptionDisabeld();

            _eventText.text = _pageTexts[localIndex];
        }

        public override void ApplyResult()
        {
            // 선택한 옵션에 따라 이벤트 초이스 할당
            var eventChoice = (_selectedOption == null) ? _mainEventData.GetEventChoice(EChoiceType.Noting) : _selectedOption.EventChoice;

            // 이벤트 결과 메시지 Log로 전달
            var resultMessage = eventChoice.GetRandomResult().resultText;
            LogManager.AddMainEventResultLog(resultMessage);
        }

        public bool ShouldEnableNextButton()
        {
            return _mainEventData == null || _mainEventData.eventType != EMainEventType.YesOrNo || _selectedOption != null;
        }

        private void UpdateChoiceOptions(MainEventData mainEventData)
        {
            AllChoiceOptionDisabeld();

            int index = 0;
            foreach (var choice in mainEventData.choices)
            {
                var iconTexture = choice.iconTexture;
                if (iconTexture == null)
                    continue;

                _choiceOptions[index].Initialize(choice);
                index++;
            }
        }

        private void AllChoiceOptionDisabeld()
        {
            foreach (var choiceOption in _choiceOptions)
            {
                choiceOption.OnSelected(false);
                choiceOption.gameObject.SetActive(false);
            }
        }
        private void AllChoiceOptionEnabled()
        {
            foreach (var choiceOption in _choiceOptions)
            {
                choiceOption.gameObject.SetActive(true);
            }
        }

        public void OnClickChoiceImage(ChoiceOption choiceOption)
        {
            if (_selectedOption != null && _selectedOption == choiceOption)
            {
                _selectedOption.OnSelected(false);
                _selectedOption = null;
            }
            else
            {
                if (_selectedOption != null)
                    _selectedOption.OnSelected(false);

                _selectedOption = choiceOption;
                _selectedOption.OnSelected(true);
            }

            ChoiceImageSelected?.Invoke();
        }
    }
}