using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive.Expedition
{
    public enum EExpeditionState
    {
        Normal,
        Ready,
        Exploring
    }

    public class ExpeditionManager : MonoSingleton<ExpeditionManager>
    {
        private const int MIN_EXPEDITION_DAY = 5;
        private const int MAX_EXPEDITION_DAY = 7;

        [SerializeField]
        private List<ExpeditionData> _expeditionDatas;

        private EExpeditionState _currentState;
        private ExpeditionData _lastExpeditionData;

        public EExpeditionState CurrentState => _currentState;

        protected override void Awake()
        {
            base.Awake();

            _currentState = EExpeditionState.Normal;
        }

        public void ReadyExpedition()
        {
            _currentState = EExpeditionState.Ready;
        }

        public void StartExpedition(CharacterBase target)
        {
            _currentState = EExpeditionState.Exploring;
            target.OnExploring();
        }

        public void CompleteExpedition()
        {
            _currentState = EExpeditionState.Normal;
        }

        public int GetRandomExpeditionDay()
        {
            return Random.Range(MIN_EXPEDITION_DAY, MAX_EXPEDITION_DAY + 1);
        }

        public ExpeditionData GetRandomExpeditionData()
        {
            if (_expeditionDatas == null || _expeditionDatas.Count == 0)
            {
                Debug.LogWarning("_expeditionDatas가 Null이거나 데이터가 존재하지 않습니다.");
                return null;
            }

            if (_expeditionDatas.Count == 1)
                return _expeditionDatas[0]; // 데이터가 하나뿐이면 어쩔 수 없이 반환

            // 최근 선택 데이터를 제외한 후보 리스트
            var candidates = _expeditionDatas
                .Where(data => data != _lastExpeditionData)
                .ToList();

            // 후보 중 랜덤 선택
            var selected = candidates[Random.Range(0, candidates.Count)];
            _lastExpeditionData = selected;

            return selected;
        }
    }
}