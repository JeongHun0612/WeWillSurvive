using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.Expedition;

namespace WeWillSurvive
{
    public class ExpeditionPanel : PagePanel
    {
        [SerializeField] private ExpeditionReadyPanel _readyPanel;
        [SerializeField] private ExpeditionSelectPanel _selectPanel;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            PanelType = EPanelType.Expedition;

            await _readyPanel.InitializeAsync();
            await _selectPanel.InitializeAsync();

            _readyPanel.gameObject.SetActive(false);
            _selectPanel.gameObject.SetActive(false);

            // 이벤트 등록
            EventBus.Subscribe<EndDayEvent>(OnEndDayEvent);

            await UniTask.CompletedTask;
        }

        public override async UniTask RefreshPageAsync(int startPageIndex)
        {
            await base.RefreshPageAsync(startPageIndex);

            EExpeditionState expeditionState = ExpeditionManager.Instance.CurrentState;

            // 플레이어 중 누군가가 탐사를 나가있는 경우 or 첫째날은 탐사 불가능
            bool isExpeditionImpossible = expeditionState == EExpeditionState.Exploring || GameManager.Instance.Day == 1;

            if (isExpeditionImpossible)
            {
                PageCount = 0;
                return;
            }

            PageCount = 1;

            if (expeditionState == EExpeditionState.Normal)
            {
                _readyPanel.gameObject.SetActive(true);
                _selectPanel.gameObject.SetActive(false);

                _readyPanel.UpdateReadyPanel();
            }
            else if (expeditionState == EExpeditionState.Ready)
            {
                _readyPanel.gameObject.SetActive(false);
                _selectPanel.gameObject.SetActive(true);

                _selectPanel.UpdateSelectPanel();
            }
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);
        }

        private void OnEndDayEvent(EndDayEvent context)
        {
            EExpeditionState expeditionState = ExpeditionManager.Instance.CurrentState;

            if (expeditionState == EExpeditionState.Normal && _readyPanel.IsReady)
            {
                ExpeditionManager.Instance.ReadyExpedition();
            }
            else if (expeditionState == EExpeditionState.Ready)
            {
                if (_selectPanel.SelectCharacter != null)
                {
                    // 탐사 시작
                    var target = _selectPanel.SelectCharacter.Owner;
                    ExpeditionManager.Instance.StartExpedition(target);
                }
            }
        }
    }
}
