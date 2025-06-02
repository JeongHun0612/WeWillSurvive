using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EThirstLevel
    {
        Normal, Thirsty, Dehydrate
    }

    public class ThirstStatus : IStatus
    {
        // 레벨별 상태 매핑
        private static readonly Dictionary<EThirstLevel, EState> _thirstyStateMap = new()
        {
            [EThirstLevel.Thirsty] = EState.Thirsty,
            [EThirstLevel.Dehydrate] = EState.Dehydrate
        };

        // 상태별 전이 기준값
        private static readonly Dictionary<EThirstLevel, float> _thirstThresholds = new()
        {
            [EThirstLevel.Normal] = 60,
            [EThirstLevel.Thirsty] = 30,
            [EThirstLevel.Dehydrate] = 0,
        };

        public EStatusType StatusType => EStatusType.Thirst;
        public float MaxValue { get; private set; } = 0f;
        public float CurrentValue { get; private set; } = 0f;
        public float DecreasePerDay { get; private set; } = 10f;

        private EThirstLevel _level = EThirstLevel.Normal;

        public ThirstStatus(float value)
        {
            MaxValue = value;
            CurrentValue = value;
        }

        public void OnNewDay(CharacterBase owner)
        {
            CurrentValue = Mathf.Max(0f, CurrentValue - DecreasePerDay);

            if (_thirstThresholds.TryGetValue(_level, out var threshold) && CurrentValue <= threshold)
            {
                if (_level == EThirstLevel.Dehydrate)
                {
                    owner.OnDead();
                    return;
                }

                _level++;
            }

            if (_thirstyStateMap.TryGetValue(_level, out var state))
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