using System.Collections.Generic;
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
        private static readonly Dictionary<EHungerLevel, Define.ECharacterState> _hungerStateMap = new()
        {
            [EHungerLevel.Hungry] = Define.ECharacterState.Hungry,
            [EHungerLevel.Starve] = Define.ECharacterState.Starve
        };

        // 상태별 전이 기준일
        private static readonly Dictionary<EHungerLevel, int> _hungerThresholds = new()
        {
            [EHungerLevel.Normal] = 2,
            [EHungerLevel.Hungry] = 2,
            [EHungerLevel.Starve] = 2,
        };


        public EStatusType StatusType => EStatusType.Hunger;
        public int DaysInState { get; private set; } = 0;

        private EHungerLevel _level = EHungerLevel.Normal;

        public void OnNewDay(CharacterBase owner)
        {
            DaysInState++;

            if (_hungerThresholds.TryGetValue(_level, out var threshold) && DaysInState >= threshold)
            {
                DaysInState = 0;

                if (_level == EHungerLevel.Starve)
                {
                    owner.OnDead();
                    return;
                }

                _level++;

                if (_hungerStateMap.TryGetValue(_level, out var state))
                {
                    owner.AddState(state);
                }
            }
        }

        public void ApplyRecovery()
        {

        }

        public void ApplyAllRecovery()
        {
            _level = EHungerLevel.Normal;
            DaysInState = 0;
        }

        public string GetStatusDescription()
        {
            return _level switch
            {
                EHungerLevel.Normal => "정상",
                EHungerLevel.Hungry => "허기짐",
                EHungerLevel.Starve => "영양 결핍",
                _ => string.Empty,
            };
        }
    }
}
