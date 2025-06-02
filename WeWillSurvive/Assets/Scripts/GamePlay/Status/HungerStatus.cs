using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EHungerLevel
    {
        Normal, Hungry, Starve
    }

    public class HungerStatus : IStatus
    {
        // 레벨별 상태 매핑
        private static readonly Dictionary<EHungerLevel, EState> _hungerStateMap = new()
        {
            [EHungerLevel.Hungry] = EState.Hungry,
            [EHungerLevel.Starve] = EState.Starve
        };

        // 상태별 전이 기준값
        private static readonly Dictionary<EHungerLevel, int> _hungerThresholds = new()
        {
            [EHungerLevel.Normal] = 60,
            [EHungerLevel.Hungry] = 30,
            [EHungerLevel.Starve] = 0,
        };

        public EStatusType StatusType => EStatusType.Hunger;
        public float MaxValue { get; private set; } = 0f;
        public float CurrentValue { get; private set; } = 0f;
        public float DecreasePerDay { get; private set; } = 10f;

        private EHungerLevel _level = EHungerLevel.Normal;

        public HungerStatus(float value)
        {
            MaxValue = value;
            CurrentValue = value;
        }

        public void OnNewDay(CharacterBase owner)
        {
            CurrentValue = Mathf.Max(0f, CurrentValue - DecreasePerDay);

            if (_hungerThresholds.TryGetValue(_level, out var threshold) && CurrentValue <= threshold)
            {
                if (_level == EHungerLevel.Starve)
                {
                    owner.OnDead();
                    return;
                }

                _level++;
            }

            if (_hungerStateMap.TryGetValue(_level, out var state))
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
