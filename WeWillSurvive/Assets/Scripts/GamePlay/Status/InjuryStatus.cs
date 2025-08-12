using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EInjuredLevel
    {
        Normal, Injured, Sick, Dead
    }

    public class InjuryStatus : StatusBase<EInjuredLevel>
    {
        public override EStatusType StatusType => EStatusType.Injury;

        protected override bool IsDeadLevel(EInjuredLevel level) => level == EInjuredLevel.Dead;

        public InjuryStatus(CharacterBase owner)
        {
            _owner = owner;
            _level = EInjuredLevel.Normal;
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

        public override void OnNewDay()
        {
            if (_level == EInjuredLevel.Normal)
                return;

            base.OnNewDay();
        }

        public override void RecoveryStatus(int step = 1)
        {
            base.RecoveryStatus(step);

            if (_level == 0)
            {
                _owner.Status.RemoveStatus(StatusType);
            }
        }
    }
}
