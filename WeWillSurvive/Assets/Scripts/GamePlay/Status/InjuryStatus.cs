using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EInjuredLevel
    {
        Normal, Injured, Sick
    }

    public class InjuryStatus : StatusBase<EInjuredLevel>
    {
        public override EStatusType StatusType => EStatusType.Injury;
        public override EBuffEffect BlockStatusBuffEffect => EBuffEffect.BlockInjuryWorsen;

        protected override bool IsDeadLevel(EInjuredLevel level) => level == EInjuredLevel.Sick;

        public InjuryStatus(CharacterBase owner)
        {
            _owner = owner;

            OrderedLevels = new EInjuredLevel[]
            {
                EInjuredLevel.Normal,
                EInjuredLevel.Injured,
                EInjuredLevel.Sick,
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
            };

            StateTransitionTable = new()
            {
                [EInjuredLevel.Normal] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 1f},
                },
                [EInjuredLevel.Injured] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.4f },
                    new StateTransition { TransitionType = EStateTransitionType.Worsen, Probability = 0.4f },
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
