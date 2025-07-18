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

        public override void OnNewDay(CharacterBase owner)
        {
            // TODO onwer �� ȥ�� ���������� ���� �ܰ��
        }

        public override void ApplyRecovery()
        {
            _owner.Status.RemoveStatus(StatusType);

            _dayCounter = 0;
        }
    }
}
