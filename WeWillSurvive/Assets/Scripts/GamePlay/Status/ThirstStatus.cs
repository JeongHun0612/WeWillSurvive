using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EThirstLevel
    {
        Normal, Thirsty, Dehydrate
    }

    public class ThirstStatus : StatusBase<EThirstLevel>
    {
        public override EStatusType StatusType => EStatusType.Thirst;

        protected override bool IsLastLevel(EThirstLevel level) => level == EThirstLevel.Dehydrate;

        public ThirstStatus(CharacterBase owner)
        {
            _owner = owner;
            _level = EThirstLevel.Normal;
            _dayCounter = 0;

            LevelStateMap = new()
            {
                [EThirstLevel.Thirsty] = EState.Thirsty,
                [EThirstLevel.Dehydrate] = EState.Dehydrate
            };

            DaysToNextLevel = new()
            {
                [EThirstLevel.Normal] = 2,
                [EThirstLevel.Thirsty] = 3,
                [EThirstLevel.Dehydrate] = 3,
            };

            StateTransitionTable = new()
            {
                [EThirstLevel.Normal] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.5f },
                    new StateTransition { TransitionType = EStateTransitionType.Worsen, Probability = 0.5f },
                },

                [EThirstLevel.Thirsty] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.8f },
                    new StateTransition { TransitionType = EStateTransitionType.Worsen, Probability = 0.2f },
                },

                [EThirstLevel.Dehydrate] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.8f },
                    new StateTransition { TransitionType = EStateTransitionType.Worsen, Probability = 0.2f },
                },
            };
        }
    }
}