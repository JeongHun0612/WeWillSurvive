using TMPro;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Expedition;
using WeWillSurvive.GameEvent;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class RationEndingPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dayCountText;
        [SerializeField] private TMP_Text _usedFoodText;
        [SerializeField] private TMP_Text _usedWaterText;
        [SerializeField] private TMP_Text _expedtionCountText;
        [SerializeField] private TMP_Text _characterEventText;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public void UpdatePanel()
        {
            _dayCountText.text = $"생존 기간 : {GameManager.Instance.Day}일";
            _usedFoodText.text = $"소모한 우주 식량 : {ItemManager.UsedFoodCount:F2}개";
            _usedWaterText.text = $"소모한 물 : {ItemManager.UsedFoodCount:F2}개";
            _expedtionCountText.text = $"수행한 탐사 횟수 : {ExpeditionManager.Instance.TotalExpeditionCount}회";
            _characterEventText.text = $"수행한 캐릭터 이벤트 횟수 : {GameEventManager.Instance.CharacterEventPicker.TotalCompleteCount}회";
        }
    }
}