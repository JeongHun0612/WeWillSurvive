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
        public override EStatusType StatusType => EStatusType.Injury;

        protected override bool IsLastLevel(EInjuredLevel level) => level == EInjuredLevel.Sick;

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
                [EInjuredLevel.Injured] = 4,
                [EInjuredLevel.Sick] = 4,
            };

            StateTransitionTable = new()
            {
                [EInjuredLevel.Injured] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.6f },
                    new StateTransition { TransitionType = EStateTransitionType.Worsen, Probability = 0.2f },
                    new StateTransition { TransitionType = EStateTransitionType.Death, Probability = 0.2f },
                },
            };
        }


        public override void ApplyRecovery()
        {
            base.ApplyRecovery();

            _owner.Status.RemoveStatus(StatusType);
        }
    }
}
