using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;

namespace WeWillSurvive.GameEvent
{
    public static class GameEventUtil
    {
        private static Dictionary<EChoiceIcon, ChoiceIconData> _choiceIconDatas = new();
        private static Dictionary<EConditionType, IEventConditionHandler> _eventConditionHandlers = new();
        private static Dictionary<EActionType, IEventActionHandler> _eventActionHandlers = new();

        private static bool _isInitalized = false;

        private static ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();
        private static ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private static CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public static async UniTask InitializeAsyn()
        {
            if (_isInitalized)
                return;

            await SetupChoiceOptionIconDatas();
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
        /// �־��� EventChoice�� ������ Ÿ�԰� �ʿ� ������ Ȯ���Ͽ� �ش� �������� Ȱ��ȭ�� �� �ִ��� ���θ� ��ȯ
        /// </summary>
        /// <param name="eventChoice"></param>
        /// <returns></returns>
        public static bool IsAvailable(EventChoice eventChoice)
        {
            if (eventChoice == null)
            {
                Debug.LogWarning("EventChoice �����Ͱ� null�Դϴ�.");
                return false;
            }

            if (Enum.TryParse($"{eventChoice.ChoiceIcon}", out ECharacter characterType))
            {
                var character = CharacterManager.GetCharacter(characterType);
                return (character != null && character.IsInShelter);
            }
            else if (Enum.TryParse($"{eventChoice.ChoiceIcon}", out EItem item))
            {
                return eventChoice.RequiredAmount == 0 || ItemManager.HasItem(item, eventChoice.RequiredAmount);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// ���޹��� EventChoice���� ��ȿ�� ���� Result�� ��ȯ
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
        public static EventResult GetValidRandomEventResult(EventChoice choice, float characterStatValue = 0f)
        {
            if (choice == null || choice.Results == null || choice.Results.Count == 0)
            {
                Debug.LogError("��ȿ���� ���� EventChoice �������Դϴ�.");
                return null;
            }

            // ������ �����Ǵ� ��� ��ȿ�� ������� ���͸�
            var validResults = choice.Results
                .Where(result => IsConditionsMet(result.Conditions))
                .ToList();

            if (validResults.Count == 0)
            {
                Debug.LogWarning("��ȿ�� EventResult�� �������� �ʽ��ϴ�.");
                return null;
            }

            // ĳ���� ���� Ȯ�� �ʿ��� ����(0.00f)�� ����
            float characterSuccessRate = Mathf.Clamp(characterStatValue, 0f, 100f) / 100.0f;

            // ���� Ȯ���� ���� ����ϰ�, ���� ����޴� ������� �������� ��ü Ȯ�� Ǯ�� ���
            List<float> finalProbabilities = new();
            float totalFixedProbability = 0f;
            foreach (var result in validResults)
            {
                if (!result.IsAffectedByStats)
                {
                    totalFixedProbability += result.Probability;
                }
            }

            if (totalFixedProbability > 1.0f)
                Debug.LogWarning($"���� Ȯ���� ���� {totalFixedProbability:P2}�� 100%�� �ʰ��߽��ϴ�.");

            float remainingProbabilityPool = Mathf.Max(0f, 1.0f - totalFixedProbability);

            // �� ����� ���� Ȯ���� ���
            foreach (var result in validResults)
            {
                float calculatedProbability = 0f;

                if (result.IsAffectedByStats)
                {
                    if (result.OutcomeType == EOutcomeType.Success)
                    {
                        calculatedProbability = remainingProbabilityPool * characterSuccessRate;
                    }
                    else if (result.OutcomeType == EOutcomeType.Failure)
                    {
                        calculatedProbability = remainingProbabilityPool * (1.0f - characterSuccessRate);
                    }
                }
                else
                {
                    calculatedProbability = result.Probability;
                }

                Debug.Log($"[{result.OutcomeType}] - Ȯ��: {calculatedProbability:P2}");
                finalProbabilities.Add(calculatedProbability);
            }

            // ���� ���� Ȯ��(����ġ)�� ���� �����ϰ� �ϳ��� ����� ����
            float totalProbability = finalProbabilities.Sum();
            float randomPoint = UnityEngine.Random.Range(0, totalProbability);

            for (int i = 0; i < validResults.Count; i++)
            {
                // ���� ����� Ȯ������ ���� ����Ʈ�� �۰ų� ������ �� ����� ����
                if (randomPoint < finalProbabilities[i])
                {
                    Debug.Log($"[{choice.Results[i].OutcomeType}] {i}��° �̺�Ʈ �߻�");
                    return choice.Results[i];
                }
                else
                {
                    // �ƴϸ�, ���� ����� Ȯ����ŭ ���� ����Ʈ�� ���̰� ���� ����� �Ѿ
                    randomPoint -= finalProbabilities[i];
                }
            }

            // ���� �ε��Ҽ��� ���� ������ ������� ���� �Ǹ� ������ ��Ҹ� ��ȯ
            return validResults[choice.Results.Count - 1];
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
        /// �̺�Ʈ ������ ������ ��ȯ
        /// </summary>
        /// <param name="choiceType"></param>
        /// <returns></returns>
        public static ChoiceIconData GetChoiceIconData(EChoiceIcon choiceType)
        {
            if (!_choiceIconDatas.TryGetValue(choiceType, out var choiceOptionIconData))
            {
                Debug.LogError($"{choiceType} Ÿ���� ChoiceIconData�� ã�� ���߽��ϴ�.");
                return null;
            }

            return choiceOptionIconData;
        }

        /// <summary>
        /// �̺�Ʈ ������ ������ ������ ����
        /// </summary>
        /// <returns></returns>
        private static async UniTask SetupChoiceOptionIconDatas()
        {
            var choiceIconDatas = await ResourceManager.LoadAssetsByLabelAsync<ChoiceIconData>("ChoiceIconData");

            foreach (var choiceIconData in choiceIconDatas)
            {
                if (!_choiceIconDatas.ContainsKey(choiceIconData.ChoiceIcon))
                {
                    _choiceIconDatas.Add(choiceIconData.ChoiceIcon, choiceIconData);
                }
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
