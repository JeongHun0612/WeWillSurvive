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

        protected int _worsenBlockDayCounter;

        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public abstract EStatusType StatusType { get; }
        public bool IsWorsenBlocked => _worsenBlockDayCounter > 0;

        protected abstract bool IsDeadLevel(TLevel level);

        public virtual void OnNewDay()
        {
            // 상태 악화 방어 버프 존재
            if (IsWorsenBlocked)
            {
                _worsenBlockDayCounter = Mathf.Max(0, _worsenBlockDayCounter - 1);
                LogStateActive(_level);
                return;
            }

            if (_owner.IsExploring)
                return;

            if (DaysToNextLevel.TryGetValue(_level, out int daysRequired) && _dayCounter >= daysRequired)
            {
                WorsenStatus();
            }

            _dayCounter++;

            // Log 추가
            LogStateActive(_level);
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
                        Debug.Log($"[{StatusType}] {stateTransition.TransitionType} 발생, 기존 레벨 {_level}");
                        HandleStateTransition(stateTransition.TransitionType);
                        break;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"{_level}에 대한 StateTransitionTable이 존재하지 않습니다.");
            }
        }

        public virtual void ResetStatus()
        {
            if (OrderedLevels == null || OrderedLevels.Length < 1)
            {
                Debug.LogWarning($"OrderedLevels 데이터가 존재하지 않습니다.");
                return;
            }

            UpdateLevel(OrderedLevels[0]);
        }

        /// <summary>
        /// 상태 악화
        /// </summary>
        /// <param name="step">상태 Level 악화 단계</param>
        public virtual void WorsenStatus(int step = 1)
        {
            if (IsWorsenBlocked)
                return;

            if (step <= 0)
                step = 1;

            int currentIndex = GetCurrentLevelIndex();
            if (currentIndex == -1)
                return;

            int targetIndex = Mathf.Min(currentIndex + step, OrderedLevels.Length - 1);
            ExecuteWorsen(targetIndex);
        }

        /// <summary>
        /// 상태 회복
        /// </summary>
        /// <param name="step">상태 Level 회복 단계</param>
        public virtual void RecoveryStatus(int step = 1)
        {
            if (step <= 0)
                step = 1;

            int currentIndex = GetCurrentLevelIndex();
            if (currentIndex == -1)
                return;

            int targetIndex = Mathf.Max(currentIndex - step, 0);
            ExecuteRecovery(targetIndex);
        }

        /// <summary>
        /// 상태 Normal로 회복
        /// </summary>
        public virtual void RecoverFully()
        {
            ExecuteRecovery(0);
        }

        public void UpdateLevel(TLevel newLevel)
        {
            // 기존 Level에 해당 State 삭제
            if (LevelStateMap.TryGetValue(_level, out var currentState))
            {
                _owner.State.RemoveState(currentState);
            }

            // 새로운 Level로 갱신
            _level = newLevel;

            // DeadLevel이면 Character 사망
            if (IsDeadLevel(_level))
            {
                _owner.OnDead();
                return;
            }

            // 변경된 Level에 해당 State 추가
            if (LevelStateMap.TryGetValue(_level, out var newState))
            {
                _owner.State.AddState(newState);
            }

            // Counter 초기화
            _dayCounter = 0;
        }

        public virtual void UpdateWorsenBlockDayCounter(int dayCounter)
        {
            _worsenBlockDayCounter = dayCounter;
        }

        public bool HasWorsenBlock() => IsWorsenBlocked;

        private void ExecuteWorsen(int targetIndex)
        {
            int currentIndex = GetCurrentLevelIndex();
            if (currentIndex == -1 || targetIndex <= currentIndex)
                return;

            // Level 갱신
            UpdateLevel(OrderedLevels[targetIndex]);

            // State에 따른 EventStateModifier 갱신
            var modifier = GetEventModifier(_level);
            modifier = Mathf.Min(modifier, _owner.EventStateModifier);
            _owner.EventStateModifier = modifier;
        }

        private void ExecuteRecovery(int targetIndex)
        {
            int currentIndex = GetCurrentLevelIndex();
            if (currentIndex == -1 || targetIndex >= currentIndex)
                return;

            // 상태 회복 Log 추가
            LogStateResolved(_level);

            // Level 갱신
            UpdateLevel(OrderedLevels[targetIndex]);

            // State에 따른 EventStateModifier 갱신
            var modifier = GetEventModifier(_level);
            modifier = Mathf.Max(modifier, _owner.EventStateModifier);
            _owner.EventStateModifier = modifier;
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

        private int GetCurrentLevelIndex()
        {
            int currentIndex = System.Array.IndexOf(OrderedLevels, _level);
            if (currentIndex == -1)
                Debug.LogWarning($"[RecoveryStatus][{StatusType}] 현재 레벨({_level})이 OrderedLevels에 존재하지 않습니다. 데이터 설정을 확인하세요.");

            return currentIndex;
        }

        private void LogStateActive(TLevel level)
        {
            if (LevelStateMap.TryGetValue(level, out var state))
            {
                string stateMessage = _owner.Data.StateMessageData.GetStateActiveMessage(state);
                LogManager.AddCharacterStatusLog(_owner.Data.Type, stateMessage);
            }
        }

        private void LogStateResolved(TLevel level)
        {
            if (LevelStateMap.TryGetValue(level, out var state))
            {
                string stateMessage = _owner.Data.StateMessageData.GetStateResolvedMessage(state);
                LogManager.AddCharacterStatusLog(_owner.Data.Type, stateMessage);
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