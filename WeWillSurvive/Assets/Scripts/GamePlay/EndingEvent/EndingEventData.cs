using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive
{
    [CreateAssetMenu(fileName = "EndingEventData", menuName = "Scriptable Objects/EndingEventData")]
    public class EndingEventData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("엔딩 이벤트가 타입")]
        private EEndingType _endingType;

        [SerializeField]
        [Tooltip("엔딩 이벤트 리스트")]
        private List<MainEventData> _endingEventDatas;

        [SerializeField]
        [Tooltip("이 엔딩 이벤트가 발생하기까지 필요한 최소 일수")]
        private int _minDayCounter;

        [SerializeField]
        [Tooltip("이 엔딩 이벤트가 발생하기까지 필요한 최대 일수")]
        private int _maxDayCounter;

        public EEndingType endingType => _endingType;
        public List<MainEventData> EndingEventDatas => _endingEventDatas;
        public int MinDayCounter => _minDayCounter;
        public int MaxDayCounter => _maxDayCounter;
    }
}