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
        [Header("## Expedition Ready Panel")]
        [SerializeField] private GameObject _expeditionReadyPanel;
        [SerializeField] private List<ExpeditionStatusPanel> _expeditionStatusPanels;
        [Header("## Expedition Ready Button")]
        [SerializeField] private Image _readyImage;
        [SerializeField] private Sprite _readyNormalSprite;
        [SerializeField] private Sprite _readySelectedSprite;

        [Header("## Expedition Select Panel")]
        [SerializeField] private GameObject _expeditionSelectPanel;
        [SerializeField] private List<ExpeditionSelectCharacter> _expeditionSelectCharacters;


        private bool _isReady;

        public async override UniTask InitializeAsync()
        {
            PanelType = EPanelType.Expedition;
            _isReady = false;

            _expeditionReadyPanel.SetActive(false);
            _expeditionSelectPanel.SetActive(false);

            await UniTask.Yield();
        }

        public override void Initialize()
        {
            PanelType = EPanelType.Expedition;
            _isReady = false;

            _expeditionReadyPanel.SetActive(false);
            _expeditionSelectPanel.SetActive(false);
        }

        public override async UniTask RefreshPageAsync(int startPageIndex)
        {
            await base.RefreshPageAsync(startPageIndex);

            EExpeditionState expeditionState = ExpeditionManager.Instance.CurrentState;

            // 플레이어 중 누군가가 탐사를 나가있는 경우 or 첫째날은 탐사 패널 X
            //bool isExpedition = expeditionState == EExpeditionState.Expedition || GameManager.Instance.Day == 1;
            bool isExpedition = expeditionState == EExpeditionState.Expedition;

            if (isExpedition)
            {
                PageCount = 0;
                return;
            }

            PageCount = 1;

            if (expeditionState == EExpeditionState.Normal)
            {
                _isReady = false;
                _expeditionReadyPanel.SetActive(true);
                _expeditionSelectPanel.SetActive(false);

                // expeditionStatusPanel 업데이트
                foreach (var expeditionStatusPanel in _expeditionStatusPanels)
                {
                    expeditionStatusPanel.UpdateExpeditionStatusPanel();
                }
            }
            else if (expeditionState == EExpeditionState.Ready)
            {
                _expeditionReadyPanel.SetActive(false);
                _expeditionSelectPanel.SetActive(true);

                // expeditionSelectCharacter 업데이트
                foreach (var expeditionSelectCharacter in _expeditionSelectCharacters)
                {
                    expeditionSelectCharacter.UpdateSelectCharacter();
                }
            }
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);
        }

        public override void ApplyResult()
        {
            if (_isReady)
                ExpeditionManager.Instance.CurrentState = EExpeditionState.Ready;

        }

        public void OnClickExpeditionReady()
        {
            _isReady = !_isReady;

            if (_isReady)
            {
                _readyImage.sprite = _readySelectedSprite;
            }
            else
            {
                _readyImage.sprite = _readyNormalSprite;
            }
        }

        public void OnClickSelectCharacter()
        {

        }
    }
}
