using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Button _nextDayButton;

        private PagePanel _currentPanel;
        private MainEventPanel _mainEventPanel;

        private int _totalPageCount;
        private int _currentPageIndex;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            // PagePanel 초기화
            foreach (var pagePanel in _pagePanels)
            {
                await pagePanel.InitializeAsync();
            }

            // PanelMoveButton 초기화
            foreach (var panelMoveButton in _panelMoveButtons)
            {
                panelMoveButton.Initialize(OnClickMovePanel);
            }

            _mainEventPanel = GetPagePanel(EPanelType.MainEvent) as MainEventPanel;
            _mainEventPanel.ChoiceImageSelected -= UpdateNextButton;
            _mainEventPanel.ChoiceImageSelected += UpdateNextButton;

            // NewDay 이벤트 등록
            EventBus.Subscribe<NewDayEvent>(OnNewDayEvent);

            await UniTask.Yield();
        }

        public override void OnShow()
        {
            base.OnShow();

            ShowCurrentPage(_currentPageIndex);
        }

        private async UniTask InitializePagePanels()
        {
            gameObject.SetActive(true);

            _totalPageCount = 0;
            _currentPageIndex = 0;
            _currentPanel = null;
            _prevButton.gameObject.SetActive(false);
            _nextDayButton.gameObject.SetActive(false);

            _dayText.text = $"Day {GameManager.Instance.Day}";

            // PanelButton 전부 활성화
            foreach (var panelMoveButton in _panelMoveButtons)
            {
                panelMoveButton.Enabled();
            }

            foreach (var pagePanel in _pagePanels)
            {
                await pagePanel.RefreshPageAsync(_totalPageCount);
                pagePanel.Hide();

                // 해당 페이지가 존재하지 않는 경우
                if (pagePanel.PageCount == 0)
                {
                    var panelMoveButton = GetPanelMoveButton(pagePanel.PanelType);
                    if (panelMoveButton != null)
                    {
                        panelMoveButton.Disabled();
                    }
                    continue;
                }

                _totalPageCount += pagePanel.PageCount;
            }

            gameObject.SetActive(false);
        }

        private void ApplyResultPanels()
        {
            foreach (var pagePanel in _pagePanels)
            {
                pagePanel.ApplyResult();
            }
        }

        private void ShowCurrentPage(int targetPageIndex)
        {
            if (targetPageIndex < 0 || targetPageIndex > _totalPageCount)
            {
                Debug.LogWarning($"페이지 범위 에러 - TargetIndex : {targetPageIndex} | Range {0} - {_totalPageCount - 1}");
                return;
            }

            foreach (var pagePanel in _pagePanels)
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
            UpdatePrevButton();
            UpdateNextButton();
        }

        private void UpdatePrevButton()
        {
            _prevButton.gameObject.SetActive(_currentPageIndex > 0);
        }
        private void UpdateNextButton()
        {
            _nextButton.gameObject.SetActive(_currentPageIndex < _totalPageCount - 1);

            if (_currentPageIndex == _totalPageCount - 1)
            {
                _nextDayButton.gameObject.SetActive(_mainEventPanel.ShouldEnableNextButton());
            }
            else
                _nextDayButton.gameObject.SetActive(false);
        }

        private void UpdatePanelMoveButton()
        {
            foreach (var panelMoveButton in _panelMoveButtons)
            {
                panelMoveButton.OnSelected(panelMoveButton.PanelType == _currentPanel.PanelType);
            }
        }

        private PanelMoveButton GetPanelMoveButton(EPanelType panelType)
        {
            foreach (var panelMoveButton in _panelMoveButtons)
            {
                if (panelMoveButton.PanelType == panelType)
                    return panelMoveButton;
            }

            return null;
        }

        private PagePanel GetPagePanel(EPanelType panelType)
        {
            return _pagePanels.FirstOrDefault(panel => panel.PanelType == panelType);
        }

        public void OnClickPrevPage() => ShowCurrentPage(_currentPageIndex - 1);
        public void OnClickNextPage() => ShowCurrentPage(_currentPageIndex + 1);
        public void OnClickMovePanel(EPanelType panelType)
        {
            foreach (var pagePanel in _pagePanels)
            {
                if (pagePanel.PanelType == panelType)
                {
                    _currentPageIndex = pagePanel.StartPageIndex;
                    ShowCurrentPage(_currentPageIndex);
                    break;
                }
            }
        }

        public void OnClickNextDay()
        {
            ApplyResultPanels();
            GameManager.Instance.StartNextDay();
        }

        private void OnNewDayEvent(NewDayEvent context)
        {
            InitializePagePanels().Forget();
        }
    }
}