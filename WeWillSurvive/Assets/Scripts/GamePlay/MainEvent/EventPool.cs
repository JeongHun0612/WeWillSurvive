using System;
using System.Collections.Generic;
using UnityEngine;

namespace WeWillSurvive.MainEvent
{
    public abstract class EventPoolBase<TEnum> : ScriptableObject where TEnum : Enum
    {
        [SerializeField] protected TEnum _category;
        [SerializeField] protected List<Condition> _conditions = new();
        [SerializeField] protected List<MainEventData> _mainEventDatas = new();
        [SerializeField, Min(0)] protected int _minDayCounter;
        [SerializeField, Min(0)] protected int _maxDayCounter;

        public TEnum Category => _category;
        public IReadOnlyList<Condition> Conditions => _conditions;
        public IReadOnlyList<MainEventData> Events => _mainEventDatas;
        public int MinDayCounter => _minDayCounter;
        public int MaxDayCounter => _maxDayCounter;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            // _minDayCounter가 _maxDayCounter 보다 크면 스왑
            if (_minDayCounter > _maxDayCounter)
                (_minDayCounter, _maxDayCounter) = (_maxDayCounter, _minDayCounter);
        }
#endif
        public int GetRandomCooldownDay()
        {
            // 리스트 비었거나 min==max==0이면 0 리턴
            if (_mainEventDatas == null || _mainEventDatas.Count == 0) 
                return 0;

            int min = Mathf.Min(_minDayCounter, _maxDayCounter);
            int max = Mathf.Max(_minDayCounter, _maxDayCounter);
            return UnityEngine.Random.Range(min, max + 1);
        }
    }

    //[CreateAssetMenu(fileName = "MainEventPool", menuName = "Scriptable Objects/MainEvent/MainEventPool")]
    //public class MainEventPool : EventPoolBase<EMainEventCategory>
    //{
    //}

    //[CreateAssetMenu(fileName = "EndingEventPool", menuName = "Scriptable Objects/MainEvent/EndingEventPool")]
    //public class EndingEventPool : EventPoolBase<EEndingType>
    //{
    //}
}
