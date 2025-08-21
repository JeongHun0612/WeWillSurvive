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
        /// 주어진 EventChoice의 아이콘 타입과 필요 조건을 확인하여 해당 선택지를 활성화할 수 있는지 여부를 반환
        /// </summary>
        /// <param name="eventChoice"></param>
        /// <returns></returns>
        public static bool IsAvailable(EventChoice eventChoice)
        {
            if (eventChoice == null)
            {
                Debug.LogWarning("EventChoice 데이터가 null입니다.");
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
        /// 전달받은 EventChoice에서 유효한 랜덤 Result를 반환
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
        public static EventResult GetValidRandomEventResult(EventChoice choice, float characterStatValue = 0f)
        {
            if (choice == null || choice.Results == null || choice.Results.Count == 0)
            {
                Debug.LogError("유효하지 않은 EventChoice 데이터입니다.");
                return null;
            }

            // 조건이 충족되는 모든 유효한 결과들을 필터링
            var validResults = choice.Results
                .Where(result => IsConditionsMet(result.Conditions))
                .ToList();

            if (validResults.Count == 0)
            {
                Debug.LogWarning("유효한 EventResult가 존재하지 않습니다.");
                return null;
            }

            // 캐릭터 성공 확률 필요한 형태(0.00f)로 가공
            float characterSuccessRate = Mathf.Clamp(characterStatValue, 0f, 100f) / 100.0f;

            // 고정 확률을 먼저 계산하고, 스탯 영향받는 결과들이 나눠가질 전체 확률 풀을 계산
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
                Debug.LogWarning($"고정 확률의 합이 {totalFixedProbability:P2}로 100%를 초과했습니다.");

            float remainingProbabilityPool = Mathf.Max(0f, 1.0f - totalFixedProbability);

            // 각 결과의 최종 확률을 계산
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

                finalProbabilities.Add(calculatedProbability);
            }

            // 계산된 최종 확률(가중치)에 따라 랜덤하게 하나의 결과를 선택
            float totalProbability = finalProbabilities.Sum();
            float randomPoint = UnityEngine.Random.Range(0, totalProbability);

            for (int i = 0; i < validResults.Count; i++)
            {
                // 현재 결과의 확률보다 랜덤 포인트가 작거나 같으면 이 결과를 선택
                if (randomPoint < finalProbabilities[i])
                {
                    Debug.Log($"[{validResults[i].OutcomeType}] {i}번째 결과 {finalProbabilities[i]:P2} 확률로 적용");
                    return validResults[i];
                }
                else
                {
                    // 아니면, 현재 결과의 확률만큼 랜덤 포인트를 줄이고 다음 결과로 넘어감
                    randomPoint -= finalProbabilities[i];
                }
            }

            // 만약 부동소수점 오류 등으로 여기까지 오게 되면 마지막 요소를 반환
            return validResults[choice.Results.Count - 1];
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
        /// 이벤트 선택지 아이콘 반환
        /// </summary>
        /// <param name="choiceType"></param>
        /// <returns></returns>
        public static ChoiceIconData GetChoiceIconData(EChoiceIcon choiceType)
        {
            if (!_choiceIconDatas.TryGetValue(choiceType, out var choiceOptionIconData))
            {
                Debug.LogError($"{choiceType} 타입의 ChoiceIconData를 찾지 못했습니다.");
                return null;
            }

            return choiceOptionIconData;
        }

        /// <summary>
        /// 이벤트 선택지 아이콘 데이터 셋팅
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
