using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive.Ending
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
        private const int MIN_GLOBAL_DAY_COUNT = 3;
        private const int MAX_GLOBAL_DAY_COUNT = 4;

        [SerializeField] private List<EndingEventPool> _endingEventPools;

        private Dictionary<EEndingType, EndingEventProgress> _endingEventProgresses = new();
        private int _globalDayCounter; // 전체 엔딩 이벤트 쿨타임 카운터

        private bool _isEndingEventReady;

        public bool IsEnding { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            SetupEndingProgress();

            ResetState();
        }

        public void ResetState()
        {
            IsEnding = false;

            _isEndingEventReady = false;
            _globalDayCounter = 0;

            foreach (var endingProgress in _endingEventProgresses.Values)
            {
                endingProgress.ResetState();
            }

            Debug.Log("EndingManager의 게임 세션 상태가 초기화되었습니다.");
        }

        public void OnNewDay()
        {
            _globalDayCounter--;

            if (_globalDayCounter <= 0)
            {
                _globalDayCounter = 0;
                _isEndingEventReady = true;
            }

            foreach (var endingEventProgress in _endingEventProgresses.Values)
            {
                endingEventProgress.OnNewDay();
            }
        }

        public MainEventData GetEndingEventData()
        {
            // 쿨타임이 아직 안됐으면 이벤트 발생 X
            if (!_isEndingEventReady)
                return null;

            // 쿨타임이 다 된 엔딩프로그래스 중 하나 반환
            EndingEventProgress progress = GetRandomCompletedEventProgress();

            // 선택된 엔딩의 이벤트 데이터 가져오기
            MainEventData eventData = progress.GetCurrentEndingEvent();

            // 상태 업데이트
            _globalDayCounter = Random.Range(MIN_GLOBAL_DAY_COUNT, MAX_GLOBAL_DAY_COUNT + 1);
            _isEndingEventReady = false;

            return eventData;
        }

        public bool AdvanceEndingProgress(EEndingType endingType)
        {
            if (_endingEventProgresses.TryGetValue(endingType, out EndingEventProgress progress))
            {
                progress.CompleteEvent();
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

        private EndingEventProgress GetRandomCompletedEventProgress()
        {
            var readyProgresses = _endingEventProgresses.Values
                .Where(progress => progress.IsReady)
                .ToList();

            if (readyProgresses.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, readyProgresses.Count);
            return readyProgresses[randomIndex];
        }

        private void SetupEndingProgress()
        {
            foreach (var endingEventPool in _endingEventPools)
            {
                if (!_endingEventProgresses.ContainsKey(endingEventPool.Category))
                {
                    _endingEventProgresses.Add(endingEventPool.Category, new EndingEventProgress(endingEventPool));
                }
            }
        }
    }
}
