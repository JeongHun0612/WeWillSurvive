using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class ExpeditionReadyPanel : MonoBehaviour
    {
        [Header("## Expedition Status Panel")]
        [SerializeField] private List<ExpeditionStatusPanel> _expeditionStatusPanels;

        [Header("## Expedition Ready Button")]
        [SerializeField] private Image _readyImage;

        private Sprite _readyNormalSprite;
        private Sprite _readySelectedSprite;

        private bool _isReady;

        public bool IsReady => _isReady;

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();


        public async UniTask InitializeAsync()
        {
            foreach (var expeditionStatusPanel in _expeditionStatusPanels)
            {
                await expeditionStatusPanel.InitializeAsync();
            }

            _readyNormalSprite = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/UI/Projecter/bag_disabled.png");
            _readySelectedSprite = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/UI/Projecter/bag.png");
        }

        public void UpdateReadyPanel()
        {
            _isReady = false;
            _readyImage.sprite = _readyNormalSprite;

            // expeditionStatusPanel 업데이트
            foreach (var expeditionStatusPanel in _expeditionStatusPanels)
            {
                expeditionStatusPanel.UpdateExpeditionStatusPanel();
            }
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
    }
}
