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

            OrderedLevels = new EInjuredLevel[]
            {
                EInjuredLevel.Normal,
                EInjuredLevel.Injured,
                EInjuredLevel.Sick,
                EInjuredLevel.Dead,
            };

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

            LevelEventModifierMap = new()
            {
                [EInjuredLevel.Normal] = 0f,
                [EInjuredLevel.Injured] = -20f,
                [EInjuredLevel.Sick] = -30f,
                [EInjuredLevel.Dead] = 0f,
            };

            StateTransitionTable = new()
            {
                [EInjuredLevel.Normal] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 1f},
                },
                [EInjuredLevel.Injured] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.6f },
                    new StateTransition { TransitionType = EStateTransitionType.Worsen, Probability = 0.2f },
                    new StateTransition { TransitionType = EStateTransitionType.Death, Probability = 0.2f },
                },
            };

            UpdateLevel(EInjuredLevel.Normal);
        }

        public override void OnNewDay()
        {
            if (_level == EInjuredLevel.Normal)
                return;

            base.OnNewDay();
        }
    }
}
