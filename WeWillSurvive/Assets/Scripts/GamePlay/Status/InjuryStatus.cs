using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EInjuredLevel
    {
        Injured, Sick
    }

    public class InjuryStatus : StatusBase<EInjuredLevel>
    {
        public override EStatusType StatusType => EStatusType.Anxious;

        public InjuryStatus(CharacterBase owner)
        {
            _owner = owner;

            _level = EInjuredLevel.Injured;
            _dayCounter = 0;

            LevelStateMap = new()
            {
                [EInjuredLevel.Injured] = EState.Injured,
                [EInjuredLevel.Sick] = EState.Sick
            };

            DaysToNextLevel = new()
            {
                [EInjuredLevel.Injured] = 3,
                [EInjuredLevel.Sick] = 2,
            };
        }

        protected override bool IsDeadLevel(EInjuredLevel level) => level == EInjuredLevel.Sick;

        public override void ApplyRecovery()
        {
            _owner.Status.RemoveStatus(StatusType);

            _dayCounter = 0;
        }
    }
}
