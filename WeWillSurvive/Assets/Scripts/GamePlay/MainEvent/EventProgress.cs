using System;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Ending;

namespace WeWillSurvive.MainEvent
{
    [System.Serializable]
    public abstract class EventProgress<TEnum> where TEnum : Enum
    {
        protected EventPoolBase<TEnum> _eventPool;

        public int DayCounter { get; protected set; }
        public bool IsReady { get; protected set; }
        public TEnum Category => _eventPool.Category;
        public IReadOnlyList<Condition> Conditions => _eventPool.Conditions;
        public IReadOnlyList<MainEventData> Events => _eventPool.Events;

        public virtual void ResetState()
        {
            DayCounter = 0;
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
        }
    }

    [System.Serializable]
    public class MainEventProgress : EventProgress<EMainEventCategory>
    {
        public MainEventProgress(EventPoolBase<EMainEventCategory> eventPool)
        {
            _eventPool = eventPool;
            ResetState();
        }

        public MainEventData GetRandomEvent()
        {
            if (!IsReady || Events == null || Events.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, Events.Count);
            return Events[randomIndex];
        }
    }

    [System.Serializable]
    public class EndingEventProgress : EventProgress<EEndingType>
    {
        public int CurrentEventIndex { get; private set; }

        public EndingEventProgress(EventPoolBase<EEndingType> eventPool)
        {
            _eventPool = eventPool;
            ResetState();
        }

        public override void ResetState()
        {
            base.ResetState();

            CurrentEventIndex = 0;
        }

        public void CompleteEvent()
        {
            CurrentEventIndex = Mathf.Min(CurrentEventIndex + 1, Events.Count - 1);

            Debug.Log($"���� [{Category}]�� ���൵�� [{CurrentEventIndex + 1}/{Events.Count}]�� ������Ʈ�Ǿ����ϴ�.");

            if (CurrentEventIndex == Events.Count - 1)
            {
                // TODO Ending �Ϸ�
            }
        }

        public MainEventData GetCurrentEndingEvent()
        {
            if (!IsReady || Events == null || Events.Count == 0)
                return null;

            ResetDayCounter();
            return Events[CurrentEventIndex];
        }
    }
}
