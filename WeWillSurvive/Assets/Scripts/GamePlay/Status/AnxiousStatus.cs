using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EAnxiousLevel
    {
        Anxious, Panic
    }

    public class AnxiousStatus : StatusBase<EAnxiousLevel>
    {
        public override EStatusType StatusType => EStatusType.Anxious;

        public AnxiousStatus(CharacterBase owner)
        {
            _owner = owner;

            _level = EAnxiousLevel.Anxious;
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
        }

        protected override bool IsDeadLevel(EAnxiousLevel level) => level == EAnxiousLevel.Panic;

        public override void OnNewDay()
        {
            // TODO onwer 가 혼자 남아있으면 다음 단계로
        }

        public override void ApplyRecovery()
        {
            base.ApplyRecovery();

            _owner.Status.RemoveStatus(StatusType);
        }
    }
}
