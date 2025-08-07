using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive
{
    public enum EEndingType
    {
        // 구조 엔딩 (Rescue Endings)
        [InspectorName("다른 탐사대 구조")]
        RescueByOtherSurvivors,

        [InspectorName("착한 외계인 구조")]
        RescueByAliens,

        [InspectorName("로봇 구조")]
        RescueByRobots,

        // 사망 엔딩 (Death Endings)
        [InspectorName("모든 캐릭터 사망")]
        DeathByStarvation,
    }

    public class EndingManager : MonoSingleton<EndingManager>
    {
        // 전체 엔딩 이벤트에 대한 전역 발생 주기
        private const int MIN_GLOBAL_DAY_COUNT = 2;
        private const int MAX_GLOBAL_DAY_COUNT = 3;

        [SerializeField] private List<EndingEventData> _endingEventDatas;

        private Dictionary<EEndingType, EndingProgress> _endingProgresses = new();
        private int _globalDayCounter; // 전체 엔딩 이벤트 쿨타임 카운터

        public bool IsEnding = false;

        protected override void Awake()
        {
            base.Awake();

            SetupEndingProgress();

            ResetState();
        }

        public void ResetState()
        {
            IsEnding = false;
            _globalDayCounter = 0;

            foreach (var endingProgress in _endingProgresses.Values)
            {
                endingProgress.ResetState();
            }

            Debug.Log("EndingManager의 게임 세션 상태가 초기화되었습니다.");
        }

        public void OnNewDay()
        {
            _globalDayCounter--;

            foreach (var endingProgress in _endingProgresses.Values)
            {
                endingProgress.dayCounter--;
            }
        }

        public MainEventData GetEndingEventData()
        {
            // 전역 쿨타임이 아직 안됐으면 이벤트 발생 X
            if (_globalDayCounter > 0)
                return null;

            // 개별 쿨타임이 다 된 엔딩이벤트들만 필터링
            List<EndingProgress> availableEndings = _endingProgresses.Values
                .Where(progress => progress.dayCounter <= 0)
                .ToList();

            // 조건에 맞는 엔딩이 없을 시
            if (availableEndings.Count == 0)
                return null;

            // 조건에 맞는 엔딩 중 랜덤 선택
            int randomIndex = Random.Range(0, availableEndings.Count);
            EndingProgress selectedEnding = availableEndings[randomIndex];

            // 선택된 엔딩의 다음 이벤트 데이터 가져오기
            int eventIndex = selectedEnding.currentEventIndex;
            MainEventData eventData = selectedEnding.EndingEventDatas[eventIndex];

            // 상태 업데이트
            _globalDayCounter = Random.Range(MIN_GLOBAL_DAY_COUNT, MAX_GLOBAL_DAY_COUNT + 1);
            selectedEnding.ResetDayCounter();

            return eventData;
        }

        public bool AdvanceEndingProgress(EEndingType endingType)
        {
            if (_endingProgresses.TryGetValue(endingType, out EndingProgress progress))
            {
                if (progress.currentEventIndex >= progress.EndingEventDatas.Count)
                {
                    Debug.LogWarning($"엔딩 [{endingType}]은(는) 이미 모든 이벤트가 진행되어 더 이상 진행할 수 없습니다.");
                    return false;
                }

                // 인덱스를 증가시켜 다음 이벤트를 가리키게 한다.
                progress.currentEventIndex++;
                Debug.Log($"엔딩 [{endingType}]의 진행도가 다음 단계({progress.currentEventIndex})로 업데이트되었습니다.");

                return true;
            }
            else
            {
                Debug.LogError($"EndingManager에 [{endingType}] 타입의 엔딩이 등록되어 있지 않습니다!");
                return false;
            }
        }

        public void Ending(EEndingType endingType)
        {
            IsEnding = true;
            // TODO 게임 엔딩
            Debug.Log($"Game Ending!! [{endingType}]");
        }

        private void SetupEndingProgress()
        {
            if (_endingProgresses == null)
                _endingProgresses = new();

            foreach (var endingEventData in _endingEventDatas)
            {
                if (!_endingProgresses.ContainsKey(endingEventData.endingType))
                {
                    _endingProgresses.Add(endingEventData.endingType, new EndingProgress(endingEventData));
                }
            }
        }
    }

    [System.Serializable]
    public class EndingProgress
    {
        private EndingEventData _eventData;

        // 현재 진행 상태
        public int currentEventIndex;
        public int dayCounter;

        public EEndingType EndingType => _eventData.endingType;
        public List<MainEventData> EndingEventDatas => _eventData.EndingEventDatas;

        public EndingProgress(EndingEventData data)
        {
            _eventData = data;
            currentEventIndex = 0;
            dayCounter = 0;
        }

        public void ResetState()
        {
            currentEventIndex = 0;
            dayCounter = 0;
        }

        public void ResetDayCounter()
        {
            dayCounter = Random.Range(_eventData.MinDayCounter, _eventData.MaxDayCounter + 1);
        }
    }
}
