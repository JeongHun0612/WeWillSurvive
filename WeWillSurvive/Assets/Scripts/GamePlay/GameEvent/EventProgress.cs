using System;
using System.Collections.Generic;
using System.Linq;

namespace WeWillSurvive.GameEvent
{
    [System.Serializable]
    public abstract class EventProgress<TEnum> where TEnum : Enum
    {
        protected EventPoolBase<TEnum> _eventPool;
        protected MainEventData _lastSelectedEvent;

        public int DayCounter { get; protected set; }
        public int EventTriggerCount { get; protected set; }
        public bool IsReady { get; protected set; }
        public TEnum Category => _eventPool.Category;
        public IReadOnlyList<Condition> Conditions => _eventPool.Conditions;
        public IReadOnlyList<MainEventData> Events => _eventPool.Events;

        protected EventProgress() { }

        public abstract MainEventData GetDailyEvent();

        public virtual void Initialize(EventPoolBase<TEnum> eventPool)
        {
            _eventPool = eventPool;
            ResetState();
        }

        public virtual void ResetState()
        {
            _lastSelectedEvent = null;
            DayCounter = 0;
            EventTriggerCount = 0;
            IsReady = false;
        }

        public virtual void OnNewDay()
        {
            if (IsReady)
                return;

            DayCounter--;

            if (DayCounter <= 0)
            {
                DayCounter = 0;
                IsReady = true;
            }
        }

        public virtual void ResetDayCounter()
        {
            DayCounter = _eventPool.GetRandomCooldownDay();
            IsReady = false;
        }

        public List<MainEventData> GetValidEvents()
        {
            if (Events == null || Events.Count == 0)
                return null;

            return Events
                .Where(mainEventData => mainEventData != _lastSelectedEvent && GameEventUtil.IsConditionsMet(mainEventData.Conditions))
                .ToList();
        }
    }
}