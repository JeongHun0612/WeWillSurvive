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
        protected TLevel[] OrderedLevels;
        protected Dictionary<TLevel, EState> LevelStateMap;
        protected Dictionary<TLevel, int> DaysToNextLevel;
        protected Dictionary<TLevel, float> LevelEventModifierMap;
        protected Dictionary<TLevel, List<StateTransition>> StateTransitionTable;

        protected CharacterBase _owner;
        protected TLevel _level;
        protected int _dayCounter;

        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public abstract EStatusType StatusType { get; }

        protected abstract bool IsDeadLevel(TLevel level);

        public virtual void OnNewDay()
        {
            _dayCounter++;

            if (DaysToNextLevel.TryGetValue(_level, out int daysRequired) && _dayCounter > daysRequired)
            {
                WorsenStatus();
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

        public virtual void ResetStatus()
        {
            if (OrderedLevels == null || OrderedLevels.Length < 1)
            {
                Debug.LogWarning($"OrderedLevels 데이터가 존재하지 않습니다.");
                return;
            }

            Debug.Log($"[{StatusType}] Reset 데이터");
            UpdateLevel(OrderedLevels[0]);
        }

        public virtual void WorsenStatus(int step = 1)
        {
            if (step <= 0)
                return;

            int currentIndex = System.Array.IndexOf(OrderedLevels, _level);
            if (currentIndex == -1)
                return;

            int targetIndex = Mathf.Min(currentIndex + step, OrderedLevels.Length - 1);
            UpdateLevel(OrderedLevels[targetIndex]);
        }

        public virtual void RecoveryStatus(int step = 1)
        {
            if (step <= 0)
                return;

            int currentIndex = System.Array.IndexOf(OrderedLevels, _level);
            if (currentIndex == -1)
                return;

            int targetIndex = Mathf.Max(currentIndex - step, 0);

            if (targetIndex < currentIndex)
            {
                // State에 따른 EventStateModifier 갱신
                var modifier = GetEventModifier(_level);
                modifier = Mathf.Max(modifier, _owner.EventStateModifier);
                _owner.EventStateModifier = modifier;

                LogStateResolved(_level);
            }

            UpdateLevel(OrderedLevels[targetIndex]);
        }

        public void UpdateLevel(TLevel newLevel)
        {
            _level = newLevel;
            _dayCounter = 0;
        }

        protected void ApplyCurrentLevelState()
        {
            if (IsDeadLevel(_level))
            {
                _owner.OnDead();
                return;
            }

            if (LevelStateMap.TryGetValue(_level, out var state))
            {
                // State 추가
                _owner.State.AddState(state);

                // State에 따른 EventStateModifier 갱신
                var modifier = GetEventModifier(_level);
                modifier = Mathf.Min(modifier, _owner.EventStateModifier);
                _owner.EventStateModifier = modifier;

                // Log 출력
                LogStateActive(_level);
            }
        }

        private float GetEventModifier(TLevel level)
        {
            if (!LevelEventModifierMap.TryGetValue(level, out var modifier))
            {
                Debug.LogWarning($"[{typeof(TLevel)}] {level}에 대한 LevelEventModifierMap이 존재하지 않습니다.");
                return 0f;
            }

            return modifier;
        }

        private void LogStateActive(TLevel level)
        {
            if (LevelStateMap.TryGetValue(level, out var state))
            {
                string stateMessage = _owner.Data.StateMessageData.GetStateActiveMessage(state);
                LogManager.AddCharacterStatusLog(_owner.Data.Type, stateMessage);
            }
            else
            {
                Debug.LogWarning($"[{typeof(TLevel)}] {level}에 대한 LevelStateMap이 존재하지 않아 로그를 기록할 수 없습니다.");
            }
        }

        private void LogStateResolved(TLevel level)
        {
            if (LevelStateMap.TryGetValue(level, out var state))
            {
                string stateMessage = _owner.Data.StateMessageData.GetStateResolvedMessage(state);
                LogManager.AddCharacterStatusLog(_owner.Data.Type, stateMessage);
            }
            else
            {
                Debug.LogWarning($"[{typeof(TLevel)}] {level}에 대한 LevelStateMap이 존재하지 않아 로그를 기록할 수 없습니다.");
            }
        }

        private void HandleStateTransition(EStateTransitionType transitionType)
        {
            switch (transitionType)
            {
                case EStateTransitionType.Stay:
                    _dayCounter = 0;
                    break;
                case EStateTransitionType.Worsen:
                    WorsenStatus();
                    break;
                case EStateTransitionType.Recovery:
                    RecoveryStatus();
                    break;
                case EStateTransitionType.Death:
                    _owner.OnDead();
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