using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Log;
using WeWillSurvive.Status;

namespace WeWillSurvive
{
    public abstract class StatusBase<TLevel> : IStatus where TLevel : System.Enum
    {
        protected Dictionary<TLevel, EState> LevelStateMap;
        protected Dictionary<TLevel, int> DaysToNextLevel;

        protected CharacterBase _owner;
        protected TLevel _level;
        protected int _dayCounter;

        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public abstract EStatusType StatusType { get; }

        protected abstract bool IsDeadLevel(TLevel level);

        public virtual void OnNewDay()
        {
            _dayCounter++;

            if (DaysToNextLevel.TryGetValue(_level, out int daysRequired) && _dayCounter >= daysRequired)
            {
                if (IsDeadLevel(_level))
                {
                    _owner.OnDead();
                    return;
                }

                _level = (TLevel)(object)((int)(object)_level + 1); // Level 증가
                _dayCounter = 0;
            }

            if (LevelStateMap.TryGetValue(_level, out var state))
            {
                // Log 에 남김
                string stateMessage = _owner.Data.GetStateActiveMessage(state);
                LogManager.AddCharacterStatusLog(_owner.Data.Type, stateMessage);

                _owner.State.AddState(state);
            }
        }

        public virtual void ApplyRecovery()
        {
            if (LevelStateMap.TryGetValue(_level, out var state))
            {
                string stateMessage = _owner.Data.GetStateResolvedMessage(state);
                LogManager.AddCharacterStatusLog(_owner.Data.Type, stateMessage);
            }

            _dayCounter = 0;
        }
    }
}