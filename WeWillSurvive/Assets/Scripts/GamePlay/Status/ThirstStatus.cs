using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EThirstLevel
    {
        Normal, Thirsty, Dehydrate, Dead
    }

    public class ThirstStatus : StatusBase<EThirstLevel>
    {
        public override EStatusType StatusType => EStatusType.Thirst;

        protected override bool IsDeadLevel(EThirstLevel level) => level == EThirstLevel.Dead;

        public ThirstStatus(CharacterBase owner)
        {
            _owner = owner;

            OrderedLevels = new EThirstLevel[]
            {
                EThirstLevel.Normal,
                EThirstLevel.Thirsty,
                EThirstLevel.Dehydrate,
                EThirstLevel.Dead,
            };

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


            LevelEventModifierMap = new()
            {
                [EThirstLevel.Normal] = 10f,
                [EThirstLevel.Thirsty] = 0f,
                [EThirstLevel.Dehydrate] = -10f,
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

            UpdateLevel(EThirstLevel.Normal);
        }
    }
}