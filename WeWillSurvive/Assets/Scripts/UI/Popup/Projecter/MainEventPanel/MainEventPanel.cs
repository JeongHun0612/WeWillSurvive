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
        [Header("## 선택 옵션 아이콘 데이터")]
        [SerializeField] private List<ChoiceOptionIconData> _choiceOptionIconDatas;

        [Header("## 이벤트 텍스트")]
        [SerializeField] private RectTransform _textRootLayout;
        [SerializeField] private TMP_Text _eventText;

        [Header("## ChoiceOption 오브젝트")]
        [SerializeField] private List<ChoiceOption> _choiceOptions;

        private Dictionary<EChoiceIcon, ChoiceOptionIconData> _choiceOptionIconDicts = new();
        private List<string> _pageTexts = new();

        private MainEventData _mainEventData = null;
        private ChoiceOption _selectedOption = null;

        private int _maxLineCount;

        public Action ChoiceImageSelected;

        public override UniTask InitializeAsync()
        {
            PanelType = EPanelType.MainEvent;

            // ChoiceOptionData Dictonary 로 정리
            _choiceOptionIconDicts.Clear();
            foreach (var choiceOptionData in _choiceOptionIconDatas)
            {
                if (!_choiceOptionIconDicts.ContainsKey(choiceOptionData.ChoiceType))
                {
                    _choiceOptionIconDicts[choiceOptionData.ChoiceType] = choiceOptionData;
                }
            }

            foreach (var choiceImage in _choiceOptions)
            {
                choiceImage.Initialize(OnClickChoiceImage);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_textRootLayout);
            _maxLineCount = TMPTextUtil.CalculateMaxLineCount(_eventText);

            return UniTask.CompletedTask;
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
            
            _pageTexts = TMPTextUtil.SplitTextByLines(_eventText, _mainEventData.GetRandomDescription(), _maxLineCount);
            PageCount = _pageTexts.Count;
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);

            _eventText.text = _pageTexts[localIndex];
        }

        public override void ApplyResult()
        {
            // 선택한 옵션에 따라 이벤트 초이스 할당
            var eventChoice = (_selectedOption == null) ? _mainEventData.GetEventChoice(EChoiceIcon.Noting) : _selectedOption.EventChoice;

            // 이벤트 결과 임시 저장
            MainEventManager.Instance.QueueEventChoiceForProcessing(eventChoice);
        }

        public bool ShouldEnableNextButton()
        {
            return _mainEventData == null || 
                (_mainEventData.ChoiceSchema != EMainEventChoiceSchema.YesOrNo && _mainEventData.ChoiceSchema != EMainEventChoiceSchema.Exploration) || 
                _selectedOption != null;
        }

        private void UpdateChoiceOptions(MainEventData mainEventData)
        {
            foreach (var choiceOption in _choiceOptions)
                choiceOption.Disabeld();

            int index = 0;
            foreach (var choice in mainEventData.Choices)
            {
                var choiceOptionIconData = GetChoiceOptionIconData(choice.ChoiceIcon);
                if (choiceOptionIconData == null)
                    continue;

                _choiceOptions[index].Initialize(choice, choiceOptionIconData);
                index++;
            }
        }

        private ChoiceOptionIconData GetChoiceOptionIconData(EChoiceIcon choiceType)
        {
            if (!_choiceOptionIconDicts.TryGetValue(choiceType, out var choiceOptionIconData))
                return null;

            return choiceOptionIconData;
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