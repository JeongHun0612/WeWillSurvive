using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Log;
using WeWillSurvive.Status;

namespace WeWillSurvive
{
    public enum EStateTransitionType
    {
        Stay,       // 상태 유지
        Worsen,     // 상태 악화
        Recovery,   // 상태 치료
        Death       // 사망
    }

    public abstract class StatusBase<TLevel> : IStatus where TLevel : System.Enum
    {
        protected Dictionary<TLevel, EState> LevelStateMap;
        protected Dictionary<TLevel, int> DaysToNextLevel;
        protected Dictionary<TLevel, List<StateTransition>> StateTransitionTable;

        protected CharacterBase _owner;
        protected TLevel _level;
        protected int _dayCounter;

        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public abstract EStatusType StatusType { get; }

        protected abstract bool IsLastLevel(TLevel level);

        public virtual void OnNewDay()
        {
            _dayCounter++;

            if (DaysToNextLevel.TryGetValue(_level, out int daysRequired) && _dayCounter > daysRequired)
            {
                AdvanceLevelOrDie();
            }

            ApplyCurrentLevelState();
        }

        public virtual void OnExpeditionResult()
        {
            float roll = Random.value;
            float cumulative = 0f;

            if (StateTransitionTable.TryGetValue(_level, out var stateTransitions))
            {
                // 확률이 초과됐을 때 경고문 발생
                float total = stateTransitions.Sum(t => t.Probability);
                if (total > 1f + 1e-5f)
                    Debug.LogWarning($"[{typeof(TLevel)}][{_level}] 확률 총합이 1.0을 초과했습니다.");

                foreach (var stateTransition in stateTransitions)
                {
                    cumulative += stateTransition.Probability;

                    if (roll < cumulative)
                    {
                        Debug.Log($"[{typeof(TLevel)}] {stateTransition.TransitionType} 발생");
                        HandleStateTransition(stateTransition.TransitionType);
                        break;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"{_level}에 대한 StateTransitionTable이 존재하지 않습니다.");
            }

            ApplyCurrentLevelState();
        }

        public virtual void ApplyRecovery()
        {
            if (LevelStateMap.TryGetValue(_level, out var state))
            {
                string stateMessage = _owner.Data.StateMessageData.GetStateResolvedMessage(state);
                LogManager.AddCharacterStatusLog(_owner.Data.Type, stateMessage);
            }
            else
            {
                Debug.LogWarning($"{_level}에 대한 LevelStateMap이 존재하지 않습니다.");
            }

            _dayCounter = 0;
        }

        protected void AdvanceLevelOrDie(int step = 1)
        {
            int currentLevel = (int)(object)_level;
            int maxLevel = System.Enum.GetValues(typeof(TLevel)).Length - 1;

            int targetLevel = currentLevel + step;

            if (targetLevel > maxLevel)
            {
                _owner.OnDead();
                return;
            }

            _level = (TLevel)(object)targetLevel;
            _dayCounter = 0;
        }

        protected void ApplyCurrentLevelState()
        {
            if (LevelStateMap.TryGetValue(_level, out var state))
            {
                // Log 출력
                string stateMessage = _owner.Data.StateMessageData.GetStateActiveMessage(state);
                LogManager.AddCharacterStatusLog(_owner.Data.Type, stateMessage);

                // State 추가
                _owner.State.AddState(state);
            }
        }

        private void HandleStateTransition(EStateTransitionType transitionType)
        {
            switch (transitionType)
            {
                case EStateTransitionType.Stay:
                    {
                        _dayCounter = 0;
                    }
                    break;
                case EStateTransitionType.Worsen:
                    {
                        AdvanceLevelOrDie();
                    }
                    break;
                case EStateTransitionType.Recovery:
                    {
                        ApplyRecovery();
                    }
                    break;
                case EStateTransitionType.Death:
                    {
                        _owner.OnDead();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class StateTransition
    {
        public EStateTransitionType TransitionType;
        public float Probability; // 0.0 ~ 1.0
    }
}