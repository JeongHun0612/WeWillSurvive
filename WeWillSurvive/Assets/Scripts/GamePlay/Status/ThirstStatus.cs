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
                [EThirstLevel.Normal] = 3,
                [EThirstLevel.Thirsty] = 2,
                [EThirstLevel.Dehydrate] = 2,
            };
        }

        protected override bool IsDeadLevel(EThirstLevel level) => level == EThirstLevel.Dehydrate;

        public override void ApplyRecovery()
        {
            base.ApplyRecovery();

            //_level = (EThirstLevel)Mathf.Max(0, (int)(object)_level - 1);

            _level = EThirstLevel.Normal;
        }
    }
}