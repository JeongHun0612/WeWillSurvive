using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Item;
using WeWillSurvive.Log;

namespace WeWillSurvive
{
    #region Enum (탐사 장소, 보상 아이템)
    public enum EExplorationLocation
    {
        [InspectorName("기지 앞 통로")] BaseFrontPath,
        [InspectorName("문서 정보관")] DocumentInformationCenter,
        [InspectorName("방재 센터")] DisasterPreventionCenter,
        [InspectorName("보안 기지")] SecurityBase,
        [InspectorName("사육 중인 원숭이 기지")] MonkeyBreedingBase,
        [InspectorName("생물 보관 구역")] BiologicalStorageArea,
        [InspectorName("에너지 주입소")] EnergyInjectionStation,
        [InspectorName("우주 더미 지역")] SpaceJunkyard,
        [InspectorName("우주 병원")] SpaceHospital,
        [InspectorName("우주 쓰레기장")] SpaceDump,
        [InspectorName("우주 학교")] SpaceSchool,
        [InspectorName("유적 기지")] RuinsBase,
        [InspectorName("자료 보관소")] DataStorage,
        [InspectorName("중력 시험장")] GravityTestSite,
        [InspectorName("지구 연합 보급선")] EarthUnionSupplyShip,
        [InspectorName("폐허 지하터널")] RuinedUndergroundTunnel
    }
    #endregion

    [CreateAssetMenu(fileName = "ExpeditionData", menuName = "Scriptable Objects/ExpeditionData")]
    public class ExpeditionData : ScriptableObject
    {
        [SerializeField] private EExplorationLocation _explorationLocation;

        [SerializeField] private List<ExpeditionRewardData> _rewardDatas = new();

        public EExplorationLocation ExplorationLocation => _explorationLocation;
        public List<ExpeditionRewardData> RewardDatas => _rewardDatas;
    }

    [System.Serializable]
    public class ExpeditionRewardData
    {
        [Tooltip("탐사 스크립트")]
        [TextArea(3, 5)]
        [SerializeField]
        private string _exploringMessage;

        [Tooltip("획득 아이템 타입")]
        [SerializeField] 
        private List<ExpeditionRewardItem> _rewardItems = new();

        public List<ExpeditionRewardItem> RewardItems => _rewardItems;
        public string ExploringMessage => _exploringMessage;

        public List<RewardItemData> GetRewardItemDatas()
        {
            var results = new List<RewardItemData>();

            foreach (var rewardItem in _rewardItems)
            {
                int amount = rewardItem.GetRandomAmount();

                if (amount <= 0)
                    continue;

                results.Add(new RewardItemData(rewardItem.RewardItem, amount));
            }

            return results;
        }
    }

    [System.Serializable]
    public class ExpeditionRewardItem
    {
        [Tooltip("획득 아이템 타입")]
        [SerializeField]
        private EItem _rewardItem;

        [Tooltip("획득 아이템 최소 개수")]
        [SerializeField]
        private int _minAmount;

        [Tooltip("획득 아이템 최대 개수")]
        [SerializeField]
        private int _maxAmount;

        public EItem RewardItem => _rewardItem;
        public int MinAmount => _minAmount;
        public int MaxAmount => _maxAmount;

        public int GetRandomAmount()
        {
            return Random.Range(_minAmount, _maxAmount + 1);
        }
    }

    [System.Serializable]
    public class ExpeditionRewardItemResult
    {
        [Tooltip("획득 아이템 타입")]
        [SerializeField]
        private EItem _rewardItem;

        [Tooltip("획득 아이템 개수")]
        [SerializeField]
        private int _amount;

        public EItem RewardItem => _rewardItem;
        public int Amount => _amount;

        public ExpeditionRewardItemResult(EItem itemType, int amount)
        {
            _rewardItem = itemType;
            _amount = amount;
        }
    }
}
