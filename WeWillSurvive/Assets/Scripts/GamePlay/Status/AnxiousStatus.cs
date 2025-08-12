using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EAnxiousLevel
    {
        Normal, Anxious, Panic, Dead
    }

    public class AnxiousStatus : StatusBase<EAnxiousLevel>
    {
        public override EStatusType StatusType => EStatusType.Anxious;

        protected override bool IsDeadLevel(EAnxiousLevel level) => level == EAnxiousLevel.Dead;


        public AnxiousStatus(CharacterBase owner)
        {
            _owner = owner;
            _level = EAnxiousLevel.Normal;
            _dayCounter = 0;

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

            StateTransitionTable = new()
            {
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
        }

        public override void OnNewDay()
        {
            ApplyCurrentLevelState();
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
