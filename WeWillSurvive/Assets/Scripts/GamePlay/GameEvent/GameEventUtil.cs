using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WeWillSurvive.GameEvent
{
    public static class GameEventUtil
    {
        private static Dictionary<EConditionType, IEventConditionHandler> _eventConditionHandlers = new();
        private static Dictionary<EActionType, IEventActionHandler> _eventActionHandlers = new();

        private static bool _isInitalized = false;

        public static void Initialize()
        {
            if (_isInitalized)
                return;

            SetupEventConditionHandlers();
            SetupEventActionHandlers();

            _isInitalized = true;
        }

        /// <summary>
        /// 전달받은 모든 조건이 충족하는지 검사
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static bool IsConditionsMet(IReadOnlyList<Condition> conditions)
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

        /// <summary>
        /// 전달받은 EventChoice에서 유효한 랜덤 Result를 반환
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
        public static EventResult GetValidRandomEventResult(EventChoice choice)
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

        /// <summary>
        /// 이벤트 조건 검사
        /// </summary>
        /// <param name="condition">이벤트 조건</param>
        /// <returns></returns>
        public static bool CheckCondition(Condition condition)
        {
            if (_eventConditionHandlers.TryGetValue(condition.ConditionType, out var handler))
            {
                return handler.IsMet(condition);
            }

            Debug.LogWarning($"핸들러가 등록되지 않은 EventCondition 타입입니다: {condition.ConditionType}");
            return false;
        }

        /// <summary>
        /// 이벤트 결과 액션
        /// </summary>
        /// <param name="action">이벤트 액션</param>
        public static void ApplyResultAction(EventAction action)
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

        /// <summary>
        /// 이벤트 조건 핸들러 셋팅
        /// </summary>
        private static void SetupEventConditionHandlers()
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

        /// <summary>
        /// 이벤트 결과 액션 핸들러 셋팅
        /// </summary>
        private static void SetupEventActionHandlers()
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
    }
}
