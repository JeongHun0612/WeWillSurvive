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
        [Header("�̺�Ʈ Ÿ��")]
        [SerializeField] protected EGameEventType _eventType;

        [Header("�̺�Ʈ �߻� ���� ���� (�ش� ��¥ ���ĺ��� �߻�)")]
        [SerializeField] protected int _availableDay;

        [Header("�̺�Ʈ�� ���� ���� �߻� �ֱ� (�߻� �ֱⰡ ���� ������ 0 ���� ����)")]
        [SerializeField] protected int _minGlobalDayCount;
        [SerializeField] protected int _maxGlobalDayCount;

        [Header("�̺�Ʈ Ǯ ����Ʈ")]
        [SerializeField] protected List<EventPoolBase<TEnum>> _eventPools = new();

        protected bool _isEventReady;                         // �̺�Ʈ �غ� �Ϸ� bool
        protected int _globalDayCounter;                      // �̺�Ʈ ��Ÿ�� ī����

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

            Debug.Log($"[{name}] ���� ���� ���°� �ʱ�ȭ�Ǿ����ϴ�.");
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
                Debug.LogError($"{category} �� �ش��ϴ� EventProgress�� ã�� ���߽��ϴ�.");
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
