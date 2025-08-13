using System;
using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EHungerLevel
    {
        Normal, Hungry, Starve, Dead
    }

    public class HungerStatus : StatusBase<EHungerLevel>
    {
        public override EStatusType StatusType => EStatusType.Hunger;

        protected override bool IsDeadLevel(EHungerLevel level) => level == EHungerLevel.Dead;

        public HungerStatus(CharacterBase owner)
        {
            _owner = owner;

            OrderedLevels = new EHungerLevel[]
            {
                EHungerLevel.Normal,
                EHungerLevel.Hungry,
                EHungerLevel.Starve,
                EHungerLevel.Dead,
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
    }
}
