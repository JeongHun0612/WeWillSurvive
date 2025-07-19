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

        public HungerStatus(CharacterBase owner)
        {
            _owner = owner;
            _level = EHungerLevel.Normal;
            _dayCounter = 0;

            LevelStateMap = new()
            {
                [EHungerLevel.Hungry] = EState.Hungry,
                [EHungerLevel.Starve] = EState.Starve
            };

            DaysToNextLevel = new()
            {
                [EHungerLevel.Normal] = 3,
                [EHungerLevel.Hungry] = 2,
                [EHungerLevel.Starve] = 2,
            };
        }

        protected override bool IsDeadLevel(EHungerLevel level) => level == EHungerLevel.Starve;

        public override void ApplyRecovery()
        {
            base.ApplyRecovery();

            //_level = (EHungerLevel)Mathf.Max(0, (int)(object)_level - 1);

            _level = EHungerLevel.Normal;
        }
    }
}
