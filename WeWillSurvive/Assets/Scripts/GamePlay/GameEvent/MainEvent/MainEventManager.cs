using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Expedition;
using WeWillSurvive.GameEvent;
using WeWillSurvive.Log;

namespace WeWillSurvive.MainEvent
{
    public class MainEventManager : MonoSingleton<MainEventManager>
    {
        [Header("## 첫째날 전용 메인 이벤트")]
        [SerializeField] private List<MainEventData> _firstDayMainEvents;

        [Header("## 메인 이벤트가 발생하지 않는 날")]
        [SerializeField] private List<MainEventData> _nothingHappensEvents = new();

        [Header("## MainEventPool")]
        [SerializeField] private List<MainEventPool> _mainEventPools = new();

        [Header("## 테스트 이벤트")]
        public MainEventData _testEventData;

        private MainEventData _lastSelectedEvent = null;
        private EventChoice _pendingEventChoice = null;

        private Dictionary<EMainEventCategory, MainEventProgress> _mainEventProgresses = new();

        private Dictionary<EConditionType, IEventConditionHandler> _eventConditionHandlers = new();
        private Dictionary<EActionType, IEventActionHandler> _eventActionHandlers = new();

        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        protected override void Awake()
        {
            base.Awake();

            SetupMainEventProgress();

            SetupEventConditionHandlers();
            SetupEventActionHandlers();
        }

        public void ResetState()
        {
            _lastSelectedEvent = null;
            _pendingEventChoice = null;
        }

        public void OnNewDay()
        {
            // 메인 이벤트 결과 적용
            ProcessPendingChoice();

            foreach (var mainEventProgress in _mainEventProgresses.Values)
            {
                mainEventProgress.OnNewDay();
            }
        }

        public MainEventData GetDailyMainEvent()
        {
            List<MainEventData> eventPool;

            // 탐사 준비 단계면 이벤트 발생 X
            bool isExpeditionReady = ExpeditionManager.Instance.CurrentState == EExpeditionState.Ready;

            if (isExpeditionReady)
            {
                var notingEvent = GetValidMainEvent(_nothingHappensEvents);
                _lastSelectedEvent = notingEvent;
                return notingEvent;
            }


            if (GameManager.Instance.Day == 1)
            {
                eventPool = _firstDayMainEvents;
            }
            else
            {
                //// 엔딩 이벤트가 발생해도 괜찮은 날이면 엔딩 이벤트 반환
                //var endingEvent = EndingManager.Instance.GetEndingEventData();
                //if (endingEvent != null)
                //{
                //    _lastSelectedEvent = endingEvent;
                //    return endingEvent;
                //}

                var eventProgress = GetRandomCompletedEventProgress();
                var mainEvents = eventProgress?.Events.ToList();

                var mainEvent = GetValidMainEvent(mainEvents);

                if (mainEvent != null)
                {
                    Debug.Log($"(메인) [{eventProgress.Category}] 이벤트 발생");
                    _lastSelectedEvent = mainEvent;
                    eventProgress.ResetDayCounter();
                    return mainEvent;
                }

                eventPool = _nothingHappensEvents;
            }

            // 결정된 목록에서 유효한 이벤트를 찾아서 반환
            var choseEvent = GetValidMainEvent(eventPool);
            _lastSelectedEvent = choseEvent;
            return choseEvent;
        }

        public void QueueEventChoiceForProcessing(EventChoice choice)
        {
            _pendingEventChoice = choice;
        }

        public void ProcessPendingChoice()
        {
            if (_pendingEventChoice == null)
            {
                Debug.Log("처리할 이벤트 선택지가 없습니다.");
                return;
            }

            EventResult result = GetEventResultFromChoice(_pendingEventChoice);

            if (result == null)
            {
                Debug.LogWarning("선택한 Choice에 대해 유효한 Result를 찾지 못했습니다. (조건불충족 또는 확률 문제)");
                return;
            }

            ApplyEventResult(result);

            _pendingEventChoice = null;
        }

        private MainEventProgress GetMainEventProgress(EMainEventCategory category)
        {
            return _mainEventProgresses.Values.FirstOrDefault(progress => progress.Category == category);
        }

        private MainEventProgress GetRandomCompletedEventProgress()
        {
            List<MainEventProgress> readyProgresses = _mainEventProgresses.Values
                .Where(progress => progress.IsReady && IsConditionsMet(progress.Conditions.ToList()))
                .ToList();

            if (readyProgresses.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, readyProgresses.Count);
            return readyProgresses[randomIndex];
        }

        private MainEventData GetValidMainEvent(IReadOnlyList<MainEventData> mainEventDatas)
        {
            if (mainEventDatas == null || mainEventDatas.Count == 0)
                return null;

            List<MainEventData> validEvents = mainEventDatas
                .Where(mainEventData => mainEventData != _lastSelectedEvent && IsConditionsMet(mainEventData.Conditions))
                .ToList();

            if (validEvents.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, validEvents.Count);
            return validEvents[randomIndex];
        }

        private EventResult GetEventResultFromChoice(EventChoice choice)
        {
            // 조건이 충족되는 모든 유효한 결과들을 필터링
            var validResults = choice.Results
                .Where(result => IsConditionsMet(result.Conditions))
                .ToList();

            if (validResults.Count == 0)
                return null;

            // 유효한 결과들의 확률 총합을 계산
            float totalProbability = validResults.Sum(result => result.Probability);

            // 확률 총합을 기반으로 랜덤 포인트를 지정
            float randomPoint = UnityEngine.Random.Range(0, totalProbability);

            // 랜덤 포인트를 사용하여 가중치 기반 랜덤 선택을 수행
            foreach (var result in validResults)
            {
                // 현재 결과의 확률보다 랜덤 포인트가 작거나 같으면 이 결과를 선택
                if (randomPoint <= result.Probability)
                {
                    return result;
                }
                else
                {
                    // 아니면, 현재 결과의 확률만큼 랜덤 포인트를 줄이고 다음 결과로 넘어감
                    randomPoint -= result.Probability;
                }
            }

            Debug.LogWarning("확률 계산 중 오류가 발생했을 수 있습니다. 유효한 결과 중 첫 번째를 반환합니다.");
            return validResults[0];
        }

        private void ApplyEventResult(EventResult eventResult)
        {
            // 이벤트 결과 적용
            foreach (var action in eventResult.Actions)
            {
                ApplyResultAction(action);
            }

            // 이벤트 결과 메시지 Log로 전달
            var resultMessage = eventResult.ResultText;
            LogManager.AddMainEventResultLog(resultMessage);
        }

        private bool IsConditionsMet(IReadOnlyList<Condition> conditions)
        {
            // 조건이 없으면 '충족'으로 간주
            if (conditions == null || conditions.Count == 0)
                return true;

            foreach (var condition in conditions)
            {
                // 하나라도 조건이 맞지 않으면 즉시 false 반환
                if (!CheckCondition(condition))
                    return false;
            }

            return true;
        }

        private void SetupMainEventProgress()
        {
            //foreach (var mainEventPool in _mainEventPools)
            //{
            //    if (!_mainEventProgresses.ContainsKey(mainEventPool.Category))
            //    {
            //        _mainEventProgresses.Add(mainEventPool.Category, new MainEventProgress(mainEventPool));
            //    }
            //}
        }

        private void SetupEventConditionHandlers()
        {
            var checkerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IEventConditionHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var checkerType in checkerTypes)
            {
                IEventConditionHandler checker = (IEventConditionHandler)Activator.CreateInstance(checkerType);

                if (!_eventConditionHandlers.ContainsKey(checker.HandledConditionType))
                {
                    _eventConditionHandlers.Add(checker.HandledConditionType, checker);
                }
            }
        }

        private void SetupEventActionHandlers()
        {
            var applicatorTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IEventActionHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var applicatorType in applicatorTypes)
            {
                IEventActionHandler applicator = (IEventActionHandler)Activator.CreateInstance(applicatorType);

                if (!_eventActionHandlers.ContainsKey(applicator.HandledActionType))
                {
                    _eventActionHandlers.Add(applicator.HandledActionType, applicator);
                }
            }
        }

        private void ApplyResultAction(EventAction action)
        {
            if (_eventActionHandlers.TryGetValue(action.ActionType, out var applicator))
            {
                applicator.Apply(action);
            }
            else
            {
                Debug.LogWarning($"핸들러가 등록되지 않은 EventAction 타입입니다: {action.ActionType}");
            }
        }

        private bool CheckCondition(Condition condition)
        {
            if (_eventConditionHandlers.TryGetValue(condition.ConditionType, out var handler))
            {
                return handler.IsMet(condition);
            }

            Debug.LogWarning($"핸들러가 등록되지 않은 EventCondition 타입입니다: {condition.ConditionType}");
            return false;
        }
    }
}
