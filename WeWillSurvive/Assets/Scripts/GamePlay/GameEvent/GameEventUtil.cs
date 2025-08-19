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
        /// ���޹��� ��� ������ �����ϴ��� �˻�
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static bool IsConditionsMet(IReadOnlyList<Condition> conditions)
        {
            // ������ ������ '����'���� ����
            if (conditions == null || conditions.Count == 0)
                return true;

            foreach (var condition in conditions)
            {
                // �ϳ��� ������ ���� ������ ��� false ��ȯ
                if (!CheckCondition(condition))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// ���޹��� EventChoice���� ��ȿ�� ���� Result�� ��ȯ
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
        public static EventResult GetValidRandomEventResult(EventChoice choice)
        {
            // ������ �����Ǵ� ��� ��ȿ�� ������� ���͸�
            var validResults = choice.Results
                .Where(result => IsConditionsMet(result.Conditions))
                .ToList();

            if (validResults.Count == 0)
                return null;

            // ��ȿ�� ������� Ȯ�� ������ ���
            float totalProbability = validResults.Sum(result => result.Probability);

            // Ȯ�� ������ ������� ���� ����Ʈ�� ����
            float randomPoint = UnityEngine.Random.Range(0, totalProbability);

            // ���� ����Ʈ�� ����Ͽ� ����ġ ��� ���� ������ ����
            foreach (var result in validResults)
            {
                // ���� ����� Ȯ������ ���� ����Ʈ�� �۰ų� ������ �� ����� ����
                if (randomPoint <= result.Probability)
                {
                    return result;
                }
                else
                {
                    // �ƴϸ�, ���� ����� Ȯ����ŭ ���� ����Ʈ�� ���̰� ���� ����� �Ѿ
                    randomPoint -= result.Probability;
                }
            }

            Debug.LogWarning("Ȯ�� ��� �� ������ �߻����� �� �ֽ��ϴ�. ��ȿ�� ��� �� ù ��°�� ��ȯ�մϴ�.");
            return validResults[0];
        }

        /// <summary>
        /// �̺�Ʈ ���� �˻�
        /// </summary>
        /// <param name="condition">�̺�Ʈ ����</param>
        /// <returns></returns>
        public static bool CheckCondition(Condition condition)
        {
            if (_eventConditionHandlers.TryGetValue(condition.ConditionType, out var handler))
            {
                return handler.IsMet(condition);
            }

            Debug.LogWarning($"�ڵ鷯�� ��ϵ��� ���� EventCondition Ÿ���Դϴ�: {condition.ConditionType}");
            return false;
        }

        /// <summary>
        /// �̺�Ʈ ��� �׼�
        /// </summary>
        /// <param name="action">�̺�Ʈ �׼�</param>
        public static void ApplyResultAction(EventAction action)
        {
            if (_eventActionHandlers.TryGetValue(action.ActionType, out var applicator))
            {
                applicator.Apply(action);
            }
            else
            {
                Debug.LogWarning($"�ڵ鷯�� ��ϵ��� ���� EventAction Ÿ���Դϴ�: {action.ActionType}");
            }
        }

        /// <summary>
        /// �̺�Ʈ ���� �ڵ鷯 ����
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
        /// �̺�Ʈ ��� �׼� �ڵ鷯 ����
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
