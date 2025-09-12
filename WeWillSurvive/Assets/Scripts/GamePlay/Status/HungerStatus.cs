using System;
using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EHungerLevel
    {
        Normal, Hungry, Starve
    }

    public class HungerStatus : StatusBase<EHungerLevel>
    {
        public override EStatusType StatusType => EStatusType.Hunger;
        public override EBuffEffect BlockStatusBuffEffect => EBuffEffect.BlockHungerWorsen;

        protected override bool IsDeadLevel(EHungerLevel level) => level == EHungerLevel.Starve;
     
        public HungerStatus(CharacterBase owner)
        {
            _owner = owner;

            OrderedLevels = new EHungerLevel[]
            {
                EHungerLevel.Normal,
                EHungerLevel.Hungry,
                EHungerLevel.Starve,
            };

            LevelStateMap = new()
            {
                [EHungerLevel.Hungry] = EState.Hungry,
                [EHungerLevel.Starve] = EState.Starve
            };

            DaysToNextLevel = new()
            {
                [EHungerLevel.Normal] = 2,
                [EHungerLevel.Hungry] = 4,
                [EHungerLevel.Starve] = 4,
            };

            LevelEventModifierMap = new()
            {
                [EHungerLevel.Normal] = 10f,
                [EHungerLevel.Hungry] = 0f,
                [EHungerLevel.Starve] = -10f,
            };

            StateTransitionTable = new()
            {
                [EHungerLevel.Normal] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.5f },
                    new StateTransition { TransitionType = EStateTransitionType.Worsen, Probability = 0.5f },
                },

                [EHungerLevel.Hungry] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.8f },
                    new StateTransition { TransitionType = EStateTransitionType.Worsen, Probability = 0.2f },
                },

                [EHungerLevel.Starve] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.8f },
                    new StateTransition { TransitionType = EStateTransitionType.Worsen, Probability = 0.2f },
                },
            };

            UpdateLevel(EHungerLevel.Normal);
        }

        public override void OnNewDay()
        {
            if (BuffManager.Instance.HasBuff(EBuffEffect.BlockAnxiousWorsen))
                return;

            base.OnNewDay();
        }
    }
}
