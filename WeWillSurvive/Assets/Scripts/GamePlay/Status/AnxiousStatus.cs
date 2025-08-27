using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive.Status
{
    public enum EAnxiousLevel
    {
        Normal, Anxious, Panic
    }

    public class AnxiousStatus : StatusBase<EAnxiousLevel>
    {
        public override EStatusType StatusType => EStatusType.Anxious;
        public override EBuffEffect BlockStatusBuffEffect => EBuffEffect.BlockAnxiousWorsen;

        protected override bool IsDeadLevel(EAnxiousLevel level) => level == EAnxiousLevel.Panic;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public AnxiousStatus(CharacterBase owner)
        {
            _owner = owner;

            OrderedLevels = new EAnxiousLevel[]
            {
                EAnxiousLevel.Normal,
                EAnxiousLevel.Anxious,
                EAnxiousLevel.Panic,
            };

            LevelStateMap = new()
            {
                [EAnxiousLevel.Anxious] = EState.Anxious,
                [EAnxiousLevel.Panic] = EState.Panic
            };

            DaysToNextLevel = new()
            {
                [EAnxiousLevel.Anxious] = 3,
                [EAnxiousLevel.Panic] = 2,
            };

            LevelEventModifierMap = new()
            {
                [EAnxiousLevel.Normal] = 0f,
                [EAnxiousLevel.Anxious] = -20f,
                [EAnxiousLevel.Panic] = -30f,
            };

            StateTransitionTable = new()
            {
                [EAnxiousLevel.Normal] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 1f },
                },
                [EAnxiousLevel.Anxious] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.7f },
                    new StateTransition { TransitionType = EStateTransitionType.Recovery, Probability = 0.2f },
                    new StateTransition { TransitionType = EStateTransitionType.Death, Probability = 0.1f },
                },
                [EAnxiousLevel.Panic] = new()
                {
                    new StateTransition { TransitionType = EStateTransitionType.Stay, Probability = 0.6f },
                    new StateTransition { TransitionType = EStateTransitionType.Recovery, Probability = 0.2f },
                    new StateTransition { TransitionType = EStateTransitionType.Death, Probability = 0.2f },
                },
            };

            UpdateLevel(EAnxiousLevel.Normal);
        }

        public override void OnNewDay()
        {
            if (_level == EAnxiousLevel.Normal)
                return;

            if (CharacterManager.InShelterCharactersCount() <= 1 && _level == EAnxiousLevel.Anxious)
            {
                WorsenStatus();
            }
        }
    }
}
