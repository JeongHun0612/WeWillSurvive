using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WeWillSurvive.GameEvent
{
    public interface IGameEventPicker
    {
        void Initialize();
        void ResetState();
        void OnNewDay();
    }

    public abstract class GameEventPickerBase<TEnum, TEventProgress> : MonoBehaviour, IGameEventPicker
        where TEnum : Enum
        where TEventProgress : EventProgress<TEnum>, new()
    {
        [Header("이벤트 타입")]
        [SerializeField] protected EGameEventType _eventType;

        [Header("이벤트 발생 기준 일자 (해당 날짜 이후부터 발생)")]
        [SerializeField] protected int _availableDay;

        [Header("이벤트에 대한 전역 발생 주기 (발생 주기가 따로 없으면 0 으로 유지)")]
        [SerializeField] protected int _minGlobalDayCount;
        [SerializeField] protected int _maxGlobalDayCount;

        [Header("이벤트 풀 리스트")]
        [SerializeField] protected List<EventPoolBase<TEnum>> _eventPools = new();

        protected bool _isEventReady;                         // 이벤트 준비 완료 bool
        protected int _globalDayCounter;                      // 이벤트 쿨타임 카운터

        protected MainEventData _lastSelectedEvent = null;
        protected Dictionary<TEnum, TEventProgress> _eventProgresses = new();

        public EGameEventType EventType => _eventType;
        public int GlobalDayCounter => _globalDayCounter;

        public virtual void Initialize()
        {
            SetupEventProgress();
            ResetState();
        }

        public virtual void ResetState()
        {
            _isEventReady = false;
            _globalDayCounter = 0;
            _lastSelectedEvent = null;

            foreach (var eventProgress in _eventProgresses.Values)
            {
                eventProgress.ResetState();
            }

            Debug.Log($"[{name}] 게임 세션 상태가 초기화되었습니다.");
        }

        public virtual void OnNewDay()
        {
            if (_isEventReady)
                return;

            _globalDayCounter--;

            if (_globalDayCounter <= 0)
            {
                _globalDayCounter = 0;
                _isEventReady = true;
            }

            foreach (var eventProgress in _eventProgresses.Values)
            {
                eventProgress.OnNewDay();
            }
        }

        public TEventProgress GetEventProgress(TEnum category)
        {
            if (!_eventProgresses.TryGetValue(category, out var progress))
            {
                Debug.LogError($"{category} 에 해당하는 EventProgress를 찾지 못했습니다.");
                return null;
            }

            return progress;
        }

        protected TEventProgress GetCompletedRandomEventProgress()
        {
            List<TEventProgress> readyProgresses = _eventProgresses.Values
                .Where(progress => progress.IsReady && GameEventUtil.IsConditionsMet(progress.Conditions.ToList()))
                .ToList();

            if (readyProgresses.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, readyProgresses.Count);
            return readyProgresses[randomIndex];
        }

        protected void ResetEventCooldown()
        {
            int min = Mathf.Min(_minGlobalDayCount, _maxGlobalDayCount);
            int max = Mathf.Max(_minGlobalDayCount, _maxGlobalDayCount);

            _globalDayCounter = UnityEngine.Random.Range(min, max + 1);
            _isEventReady = false;
        }

        private void SetupEventProgress()
        {
            foreach (var eventPool in _eventPools)
            {
                if (!_eventProgresses.ContainsKey(eventPool.Category))
                {
                    var progress = new TEventProgress();
                    progress.Initialize(eventPool);
                    _eventProgresses.Add(eventPool.Category, progress);
                }
            }
        }
    }
}
