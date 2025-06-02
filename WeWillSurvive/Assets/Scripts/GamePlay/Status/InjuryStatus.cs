using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EInjuredLevel
    {
        Injured, Sick
    }

    public class InjuryStatus : IStatus
    {
        // 레벨별 상태 매핑
        private static readonly Dictionary<EInjuredLevel, EState> _injuryStateMap = new()
        {
            [EInjuredLevel.Injured] = EState.Injured,
            [EInjuredLevel.Sick] = EState.Sick
        };

        // 상태별 전이 기준값
        private static readonly Dictionary<EInjuredLevel, int> _injuryThresholds = new()
        {
            [EInjuredLevel.Injured] = 50,
            [EInjuredLevel.Sick] = 0,
        };

        public EStatusType StatusType => EStatusType.Injury;

        public float MaxValue { get; private set; } = 0f;

        public float CurrentValue { get; private set; } = 0f;

        public float DecreasePerDay { get; private set; } = 10f;

        private EInjuredLevel _level = EInjuredLevel.Injured;

        public InjuryStatus(float value)
        {
            MaxValue = value;
            CurrentValue = value;
        }

        public void OnNewDay(CharacterBase owner)
        {
            CurrentValue = Mathf.Max(0f, CurrentValue - DecreasePerDay);

            if (_injuryThresholds.TryGetValue(_level, out var threshold) && CurrentValue <= threshold)
            {
                if (_level == EInjuredLevel.Sick)
                {
                    owner.OnDead();
                    return;
                }

                _level++;
            }

            if (_injuryStateMap.TryGetValue(_level, out var state))
            {
                owner.State.AddState(state);
            }
        }

        public void ApplyRecovery(float value)
        {
            CurrentValue = Mathf.Min(MaxValue, CurrentValue + value);
        }
    }
}
