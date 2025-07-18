using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive
{
    public class MainEventPanel : PagePanel
    {
        [Header("## Event Text")]
        [SerializeField] private TMP_Text _eventText;
        [SerializeField] private TMP_Text _tempText;
        [SerializeField] private int _maxLineCount;

        [Header("## Choice Objects")]
        [SerializeField] private List<ChoiceOption> _choiceOptions;

        private List<string> _pageTexts = new();

        private MainEventData _mainEventData = null;
        private ChoiceOption _selectedOption = null;

        public Action ChoiceImageSelected;

        public override void Initialize()
        {
            PanelType = EPanelType.MainEvent;

            foreach (var choiceImage in _choiceOptions)
            {
                choiceImage.Initialize(OnClickChoiceImage);
            }
        }

        public override async UniTask RefreshPageAsync(int startPageIndex)
        {
            await base.RefreshPageAsync(startPageIndex);

            _mainEventData = MainEventManager.Instance.DebugEventData;
            _selectedOption = null;

            // 선택지 업데이트
            UpdateChoiceOptions(_mainEventData);

            gameObject.SetActive(true);
            await UniTask.NextFrame();

            _pageTexts = TMPTextUtil.SplitTextByLines(_tempText, _mainEventData.descriptions[0], _maxLineCount);
            PageCount = _pageTexts.Count;
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);

            _eventText.text = _pageTexts[localIndex];
        }

        public override void ApplyResult()
        {
            if (_selectedOption == null)
            {
                // 선택하지 않음
                //_notingChoice.results
                //choice.results.
            }
            else
            {
                //_selectedOption.EventChoice.results
            }

            //_mainEventData.eventType
        }

        public bool ShouldEnableNextButton()
        {
            return _mainEventData == null || _mainEventData.eventType != EMainEventType.YesOrNo || _selectedOption != null;
        }

        private void UpdateChoiceOptions(MainEventData mainEventData)
        {
            foreach (var choiceOption in _choiceOptions)
            {
                choiceOption.OnSelected(false);
                choiceOption.gameObject.SetActive(false);
            }

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