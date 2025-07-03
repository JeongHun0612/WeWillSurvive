using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_Projecter : UI_Popup
    {
        [Header("## Day Text")]
        [SerializeField] private TMP_Text _dayText;

        [Header("## Page Panel")]
        [SerializeField] private List<PagePanel> _pagePanels;

        [Header("## PaenlMove Button")]
        [SerializeField] private List<PanelMoveButton> _panelMoveButtons;

        [Header("## Page Move Button")]
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _nextButton;

        private Dictionary<EPanelType, PagePanel> _pagePanelDicts = new();
        private Dictionary<EPanelType, PanelMoveButton> _panelMoveButtonDicts = new();

        private PagePanel _currentPanel;
        private PanelMoveButton _selectedPanelMoveButton;

        private int _totalPageCount;
        private int _currentPageIndex;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            // PagePanel 초기화
            foreach (var pagePanel in _pagePanels)
            {
                pagePanel.Initialize();

                if (!_pagePanelDicts.ContainsKey(pagePanel.PanelType))
                {
                    _pagePanelDicts.Add(pagePanel.PanelType, pagePanel);
                }
                else
                {
                    _pagePanelDicts[pagePanel.PanelType] = pagePanel;
                }
            }

            // PanelMoveButton 초기화
            foreach (var panelMoveButton in _panelMoveButtons)
            {
                panelMoveButton.Initialize(OnClickMovePanel);

                if (!_panelMoveButtonDicts.ContainsKey(panelMoveButton.PanelType))
                {
                    _panelMoveButtonDicts.Add(panelMoveButton.PanelType, panelMoveButton);
                }
                else
                {
                    _panelMoveButtonDicts[panelMoveButton.PanelType] = panelMoveButton;
                }
            }

            // NewDay 이벤트 등록
            EventBus.Subscribe<NewDayEvent>(OnNewDayEvent);

            // Page 초기화 및 할당
            InitializePagePanels();

            await UniTask.Yield();
        }

        public override void OnShow()
        {
            base.OnShow();

            ShowCurrentPage(_currentPageIndex);
        }

        private void InitializePagePanels()
        {
            gameObject.SetActive(true);

            _totalPageCount = 0;
            _currentPageIndex = 0;
            _currentPanel = null;
            _selectedPanelMoveButton = null;

            _dayText.text = $"Day {GameManager.Instance.Day}";

            foreach (var pagePanel in _pagePanelDicts.Values)
            {
                pagePanel.InitializePage(_totalPageCount);
                pagePanel.Hide();

                // 해당 페이지가 존재하지 않는 경우
                if (pagePanel.PageCount == 0)
                {
                    if (_panelMoveButtonDicts.TryGetValue(pagePanel.PanelType, out var panelMoveButton))
                    {
                        panelMoveButton.Disabled();
                    }

                    continue;
                }

                _totalPageCount += pagePanel.PageCount;
            }

            gameObject.SetActive(false);
        }

        private void ShowCurrentPage(int targetPageIndex)
        {
            if (targetPageIndex < 0 || targetPageIndex >= _totalPageCount)
            {
                Debug.LogWarning($"페이지 범위 에러 - TargetIndex : {targetPageIndex} | Range {0} - {_totalPageCount - 1}");
                return;
            }

            foreach (var pagePanel in _pagePanelDicts.Values)
            {
                if (!pagePanel.HasPage(targetPageIndex))
                    continue;

                if (_currentPanel != null && _currentPanel != pagePanel)
                    _currentPanel.Hide();

                _currentPanel = pagePanel;

                int localIndex = targetPageIndex - pagePanel.StartPageIndex;
                _currentPanel.ShowPage(localIndex);

                break;
            }

            _currentPageIndex = targetPageIndex;

            UpdateMoveButton();
            UpdatePanelMoveButton();
        }



        private void UpdateMoveButton()
        {
            _prevButton.interactable = _currentPageIndex > 0;
            _nextButton.interactable = _currentPageIndex < _totalPageCount - 1;
        }

        private void UpdatePanelMoveButton()
        {
            if (_currentPanel == null)
                return;

            if (!_panelMoveButtonDicts.TryGetValue(_currentPanel.PanelType, out var panelMoveButton))
            {
                Debug.LogError($"{_currentPanel.PanelType} Type의 PanelMoveButton이 존재하지 않습니다.");
                return;
            }

            if (panelMoveButton == _selectedPanelMoveButton)
                return;

            _selectedPanelMoveButton?.OnSelected(false);
            panelMoveButton.OnSelected(true);

            _selectedPanelMoveButton = panelMoveButton;
        }

        public void OnClickPrevPage() => ShowCurrentPage(_currentPageIndex - 1);
        public void OnClickNextPage() => ShowCurrentPage(_currentPageIndex + 1);
        public void OnClickMovePanel(EPanelType panelType)
        {
            if (!_pagePanelDicts.TryGetValue(panelType, out var pagePanel))
            {
                Debug.LogError($"{panelType} Type의 Panel이 존재하지 않습니다.");
                return;
            }

            _currentPageIndex = pagePanel.StartPageIndex;
            ShowCurrentPage(_currentPageIndex);
        }

        private void OnNewDayEvent(NewDayEvent context)
        {
            InitializePagePanels();
        }
    }
}
