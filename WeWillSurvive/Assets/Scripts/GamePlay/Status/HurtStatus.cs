using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EHurtLevel
    {
        Hurt, Sick
    }

    public class HurtStatus : IStatus
    {
        // 레벨별 상태 매핑
        private static readonly Dictionary<EHurtLevel, Define.ECharacterState> _hurtStateMap = new()
        {
            [EHurtLevel.Hurt] = Define.ECharacterState.Hurt,
            [EHurtLevel.Sick] = Define.ECharacterState.Sick
        };

        // 상태별 전이 기준일
        private static readonly Dictionary<EHurtLevel, int> _hurtThresholds = new()
        {
            [EHurtLevel.Hurt] = 2,
            [EHurtLevel.Sick] = 2,
        };

        public EStatusType StatusType => EStatusType.Hurt;

        public int DaysInState { get; private set; } = 0;

        private EHurtLevel _level = EHurtLevel.Hurt;

        public void OnNewDay(CharacterBase owner)
        {
            DaysInState++;

            if (_hurtThresholds.TryGetValue(_level, out var threshold) && DaysInState >= threshold)
            {
                DaysInState = 0;

                if (_level == EHurtLevel.Sick)
                {
                    owner.OnDead();
                    return;
                }

                _level++;

                if (_hurtStateMap.TryGetValue(_level, out var state))
                {
                    owner.AddState(state);
                }
            }
        }

        public void ApplyRecovery()
        {
        }

        public string GetStatusDescription()
        {
            return _level switch
            {
                EHurtLevel.Hurt => "다침",
                EHurtLevel.Sick => "병듦",
                _ => string.Empty,
            };
        }
    }
}
