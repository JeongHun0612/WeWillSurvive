using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EAnxiousLevel
    {
        Anxious, Panic
    }

    public class AnxiousStatus : IStatus
    {
        // 레벨별 상태 매핑
        private static readonly Dictionary<EAnxiousLevel, EState> _anxiousStateMap = new()
        {
            [EAnxiousLevel.Anxious] = EState.Anxious,
            [EAnxiousLevel.Panic] = EState.Panic
        };

        // 상태별 전이 기준값
        private static readonly Dictionary<EAnxiousLevel, int> _anxiousThresholds = new()
        {
            [EAnxiousLevel.Anxious] = 30,
            [EAnxiousLevel.Panic] = 0,
        };

        public EStatusType StatusType => EStatusType.Anxious;
        public float MaxValue { get; private set; } = 0f;
        public float CurrentValue { get; private set; } = 0f;
        public float DecreasePerDay { get; private set; } = 10f;

        private EAnxiousLevel _level = EAnxiousLevel.Anxious;

        public AnxiousStatus(float value)
        {
            MaxValue = value;
            CurrentValue = value;
        }

        public void OnNewDay(CharacterBase owner)
        {
            CurrentValue = Mathf.Max(0f, CurrentValue - DecreasePerDay);

            if (_anxiousThresholds.TryGetValue(_level, out var threshold) && CurrentValue <= threshold)
            {
                if (_level == EAnxiousLevel.Panic)
                {
                    owner.OnDead();
                    return;
                }

                _level++;
            }

            if (_anxiousStateMap.TryGetValue(_level, out var state))
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
