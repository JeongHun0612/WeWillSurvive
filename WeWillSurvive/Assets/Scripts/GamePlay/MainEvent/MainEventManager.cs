using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Expedition;
using WeWillSurvive.Item;
using WeWillSurvive.Log;

namespace WeWillSurvive.MainEvent
{
    public class MainEventManager : MonoSingleton<MainEventManager>
    {
        [Header("## 첫째날 전용 메인 이벤트")]
        [SerializeField] private List<MainEventData> _firstDayMainEventDatas;

        [Header("## 메인 이벤트가 발생하지 않는 날")]
        [SerializeField] private List<MainEventData> _nothingHappensEventDatas = new();

        [Header("## MainEventData")]
        public List<MainEventData> _mainEventDatas = new();


        [Header("## 테스트 이벤트")]
        public MainEventData _testEventData;

        private MainEventData _lastSelectedEvent = null;
        private EventChoice _pendingEventChoice = null;

        private Dictionary<EConditionType, IEventConditionHandler> _eventConditionHandlers = new();
        private Dictionary<EActionType, IEventActionHandler> _eventActionHandlers = new();

        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        protected override void Awake()
        {
            base.Awake();

            SetupEventConditionHandlers();
            SetupEventActionHandlers();
        }

        public void ResetState()
        {
            _lastSelectedEvent = null;
            _pendingEventChoice = null;
        }

        public MainEventData GetDailyMainEvent()
        {
            if (_testEventData != null)
            {
                IsConditionsMet(_testEventData.triggerConditions);

                return _testEventData;
            }

            List<MainEventData> eventPool;

            if (GameManager.Instance.Day == 1)
            {
                eventPool = _firstDayMainEventDatas;
            }
            else
            {
                // 엔딩 이벤트가 발생해도 괜찮은 날이면 엔딩 이벤트 반환
                var endingEvent = EndingManager.Instance.GetEndingEventData();
                if (endingEvent != null)
                {
                    _lastSelectedEvent = endingEvent;
                    return endingEvent;
                }

                // 탐사 준비 단계가 아니면 일반 이벤트, 맞으면 아무일도 없는 이벤트
                bool isExpeditionReady = ExpeditionManager.Instance.CurrentState == EExpeditionState.Ready;
                eventPool = isExpeditionReady ? _nothingHappensEventDatas : _mainEventDatas;
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

        private MainEventData GetValidMainEvent(List<MainEventData> mainEventDatas)
        {
            List<MainEventData> validEvents = mainEventDatas
                .Where(mainEventData => mainEventData != _lastSelectedEvent && IsConditionsMet(mainEventData.triggerConditions))
                .ToList();

            if (validEvents.Count == 0)
            {
                if (_lastSelectedEvent != null && IsConditionsMet(_lastSelectedEvent.triggerConditions))
                {
                    return _lastSelectedEvent;
                }
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, validEvents.Count);
            return validEvents[randomIndex];
        }

        private EventResult GetEventResultFromChoice(EventChoice choice)
        {
            // 조건이 충족되는 모든 유효한 결과들을 필터링
            var validResults = choice.results
                .Where(result => IsConditionsMet(result.conditions))
                .ToList();

            if (validResults.Count == 0)
                return null;

            // 유효한 결과들의 확률 총합을 계산
            float totalProbability = validResults.Sum(result => result.probability);

            // 확률 총합을 기반으로 랜덤 포인트를 지정
            float randomPoint = UnityEngine.Random.Range(0, totalProbability);

            // 랜덤 포인트를 사용하여 가중치 기반 랜덤 선택을 수행
            foreach (var result in validResults)
            {
                // 현재 결과의 확률보다 랜덤 포인트가 작거나 같으면 이 결과를 선택
                if (randomPoint <= result.probability)
                {
                    return result;
                }
                else
                {
                    // 아니면, 현재 결과의 확률만큼 랜덤 포인트를 줄이고 다음 결과로 넘어감
                    randomPoint -= result.probability;
                }
            }

            Debug.LogWarning("확률 계산 중 오류가 발생했을 수 있습니다. 유효한 결과 중 첫 번째를 반환합니다.");
            return validResults[0];
        }

        private void ApplyEventResult(EventResult eventResult)
        {
            // 이벤트 결과 적용
            foreach (var action in eventResult.actions)
            {
                ApplyResultAction(action);
            }

            // 이벤트 결과 메시지 Log로 전달
            var resultMessage = eventResult.resultText;
            LogManager.AddMainEventResultLog(resultMessage);
        }

        private bool IsConditionsMet(List<Condition> conditions)
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
            if (_eventActionHandlers.TryGetValue(action.actionType, out var applicator))
            {
                applicator.Apply(action);
            }
            else
            {
                Debug.LogWarning($"핸들러가 등록되지 않은 EventAction 타입입니다: {action.actionType}");
            }
        }

        private bool CheckCondition(Condition condition)
        {
            if (_eventConditionHandlers.TryGetValue(condition.conditionType, out var handler))
            {
                return handler.IsMet(condition);
            }

            Debug.LogWarning($"핸들러가 등록되지 않은 EventCondition 타입입니다: {condition.conditionType}");
            return false;
        }
    }
}
