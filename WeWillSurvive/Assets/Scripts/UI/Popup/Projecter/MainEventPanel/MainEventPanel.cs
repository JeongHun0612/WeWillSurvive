using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Ending;
using WeWillSurvive.GameEvent;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class MainEventPanel : PagePanel
    {
        [Header("## 이벤트 텍스트")]
        [SerializeField] private RectTransform _textRootLayout;
        [SerializeField] private TMP_Text _eventText;

        [Header("## ChoiceOption 오브젝트")]
        [SerializeField] private List<ChoiceOption> _choiceOptions;

        private List<string> _pageTexts = new();

        private MainEventData _mainEventData = null;
        private ChoiceOption _selectedOption = null;

        private int _maxLineCount;

        public Action ChoiceImageSelected;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public async override UniTask InitializeAsync()
        {
            PanelType = EPanelType.MainEvent;

            foreach (var choiceImage in _choiceOptions)
            {
                choiceImage.Initialize(OnClickChoiceImage);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_textRootLayout);

            _maxLineCount = TMPTextUtil.CalculateMaxLineCount(_eventText);

            // 이벤트 등록
            EventBus.Subscribe<EndDayEvent>(OnEndDayEvent);
            EventBus.Subscribe<RationItemSelectedEvent>(OnRationItemSelectedEvent);

            await UniTask.CompletedTask;
        }

        public override async UniTask RefreshPageAsync(int startPageIndex)
        {
            await base.RefreshPageAsync(startPageIndex);

            _selectedOption = null;
            _mainEventData = GameEventManager.Instance.DailyMainEvent?.DailyEventData;

            if (_mainEventData == null || EndingManager.Instance.IsEnding)
            {
                PageCount = 0;
                return;
            }

            // 선택지 업데이트
            UpdateChoiceOptions(_mainEventData);

            // 페이지 수 계산
            gameObject.SetActive(true);
            await UniTask.NextFrame();

            var decription = _mainEventData.GetRandomDescription();
            if (string.IsNullOrWhiteSpace(decription))
            {
                PageCount = 1;
                return;
            }

            _pageTexts = TMPTextUtil.SplitTextByLines(_eventText, _mainEventData.GetRandomDescription(), _maxLineCount);
            PageCount = _pageTexts.Count;
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);

            _eventText.text = _pageTexts[localIndex];
        }

        public bool ShouldEnableNextButton()
        {
            if (_mainEventData == null || !_mainEventData.IsChoiceRequired || _selectedOption != null)
                return true;

            bool isAnyChoiceAvailable = _mainEventData.Choices.Any(choice =>
            {
                if (Enum.TryParse<EItem>($"{choice.ChoiceIcon}", out var item))
                {
                    return ItemManager.HasItem(item);
                }

                return false;
            });

            return !isAnyChoiceAvailable;
        }

        private void UpdateChoiceOptions(MainEventData mainEventData)
        {
            foreach (var choiceOption in _choiceOptions)
                choiceOption.ResetState();

            int index = 0;
            foreach (var choice in mainEventData.Choices)
            {
                if (choice.ChoiceIcon == EChoiceIcon.Noting)
                    continue;

                _choiceOptions[index].UpdateChoiceOption(choice);
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

        private void OnEndDayEvent(EndDayEvent context)
        {
            // 선택한 옵션에 따라 이벤트 초이스 할당
            var eventChoice = (_selectedOption == null) ? _mainEventData.GetEventChoice(EChoiceIcon.Noting) : _selectedOption.EventChoice;

            // 이벤트 결과 임시 저장
            GameEventManager.Instance.SelectedMainEventChoice(eventChoice);
        }

        private void OnRationItemSelectedEvent(RationItemSelectedEvent context)
        {
            foreach (var choiceOption in _choiceOptions)
            {
                if (choiceOption.EventChoice == null)
                    continue;

                if (Enum.TryParse($"{choiceOption.EventChoice.ChoiceIcon}", out EItem item) && item == context.Item)
                {
                    choiceOption.UpdateRemainItemCount(context.RemainCount);
                    choiceOption.UpdateAvailable();
                }
            }
        }
    }
}