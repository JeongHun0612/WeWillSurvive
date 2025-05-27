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
        private static readonly Dictionary<EThirstLevel, Define.ECharacterState> _thirstyStateMap = new()
        {
            [EThirstLevel.Thirsty] = Define.ECharacterState.Thirsty,
            [EThirstLevel.Dehydrate] = Define.ECharacterState.Dehydrate
        };

        // 상태별 전이 기준일
        private static readonly Dictionary<EThirstLevel, int> _thirstThresholds = new()
        {
            [EThirstLevel.Normal] = 2,
            [EThirstLevel.Thirsty] = 2,
            [EThirstLevel.Dehydrate] = 2,
        };

        public EStatusType StatusType => EStatusType.Thirst;
        public int DaysInState { get; private set; } = 0;

        private EThirstLevel _level = EThirstLevel.Normal;

        public void OnNewDay(CharacterBase owner)
        {
            DaysInState++;

            if (_thirstThresholds.TryGetValue(_level, out var threshold) && DaysInState >= threshold)
            {
                DaysInState = 0;

                if (_level == EThirstLevel.Dehydrate)
                {
                    owner.OnDead();
                    return;
                }

                _level++;

                if (_thirstyStateMap.TryGetValue(_level, out var state))
                {
                    owner.AddState(state);
                }
            }
        }

        public void ApplyRecovery()
        {
            if (_level > EThirstLevel.Normal)
            {
                _level--;
            }

            DaysInState = 0;
        }

        public string GetStatusDescription()
        {
            return _level switch
            {
                EThirstLevel.Normal => "정상",
                EThirstLevel.Thirsty => "갈증",
                EThirstLevel.Dehydrate => "수분 고갈",
                _ => string.Empty,
            };
        }

    }
}
