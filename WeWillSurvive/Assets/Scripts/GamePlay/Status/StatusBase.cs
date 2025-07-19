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

        public abstract void ApplyRecovery();
        protected abstract bool IsDeadLevel(TLevel level);

        public virtual void OnNewDay(CharacterBase owner)
        {
            _dayCounter++;

            if (DaysToNextLevel.TryGetValue(_level, out int daysRequired) && _dayCounter >= daysRequired)
            {
                if (IsDeadLevel(_level))
                {
                    owner.OnDead();
                    return;
                }

                _level = (TLevel)(object)((int)(object)_level + 1); // Level ¡ı∞°
                _dayCounter = 0;
            }

            if (LevelStateMap.TryGetValue(_level, out var state))
            {
                // Log ø° ≥≤±Ë
                string stateMessage = owner.Data.GetStateActiveMessage(state);
                LogManager.AddCharacterStatusLog(owner.Data.Type, stateMessage);

                owner.State.AddState(state);
            }
        }
    }
}
