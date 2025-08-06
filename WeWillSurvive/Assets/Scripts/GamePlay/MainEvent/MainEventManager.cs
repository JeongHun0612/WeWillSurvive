using System;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Expedition;
using WeWillSurvive.Item;
using WeWillSurvive.Log;
using WeWillSurvive.Util;

namespace WeWillSurvive.MainEvent
{
    public class MainEventManager : MonoSingleton<MainEventManager>
    {
        public List<MainEventData> _testDatas = new();

        [Header("## 첫째날 전용 메인 이벤트")]
        [SerializeField] private List<MainEventData> _firstDayMainEventDatas;

        [Header("## 메인 이벤트가 발생하지 않는 날")]
        [SerializeField] private List<MainEventData> _notingMainEventDatas = new();

        [Header("## MainEventData")]
        public List<MainEventData> _debugMainEventDatas = new();

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        protected override void Awake()
        {
            base.Awake();

        }

        public MainEventData GetDailyMainEvent()
        {
            // 탐사 준비 단계이면 메인 이벤트 X
            bool isDailyMainEvent = !(ExpeditionManager.Instance.CurrentState == EExpeditionState.Ready);

            if (GameManager.Instance.Day == 1)
            {
                return GetValidMainEvent(_firstDayMainEventDatas);
            }

            if (!isDailyMainEvent)
            {
                return GetValidMainEvent(_notingMainEventDatas);
            }
            else
            {
                return GetValidMainEvent(_debugMainEventDatas);
            }
        }

        public void SetMainEventResult(EventResult eventResult)
        {
            // 이벤트 결과 적용
            var resultItemDatas = new List<ResultItemData>();
            foreach (var effect in eventResult.effects)
            {
                ApplyResultEffect(effect);

                if (effect.effectType == EEffectType.AddItem || effect.effectType == EEffectType.RemoveItem)
                {
                    EItem itemType = Enum.Parse<EItem>(effect.targetId);
                    int amount = int.Parse(effect.value);

                    // RemoveItem 타입이면 음수로 전달
                    if (effect.effectType == EEffectType.RemoveItem)
                        amount = -amount;

                    resultItemDatas.Add(new ResultItemData(itemType, amount));
                }
            }

            // 이벤트 결과 메시지 Log로 전달
            var resultMessage = eventResult.resultText;
            LogManager.AddMainEventResultLog(resultMessage, resultItemDatas);
        }

        private void ApplyResultEffect(EventEffect effect)
        {
            switch (effect.effectType)
            {
                case EEffectType.AddItem:
                    {
                        EItem item = Enum.Parse<EItem>(effect.targetId);
                        float count = int.Parse(effect.value);
                        ItemManager.AddItem(item, count);

                        //ResultItemData
                    }
                    break;
                case EEffectType.RemoveItem:
                    {
                        EItem item = Enum.Parse<EItem>(effect.targetId);
                        float count = int.Parse(effect.value);
                        ItemManager.UsedItem(item, count);
                    }
                    break;
                case EEffectType.IncreaseStatus:
                    break;
                case EEffectType.DecreaseStatus:
                    break;
                default:
                    break;
            }
        }

        private MainEventData GetValidMainEvent(List<MainEventData> mainEventDatas)
        {
            List<MainEventData> validEvents = new();

            foreach (var mainEventData in mainEventDatas)
            {
                bool isAllConditionCheck = true;

                foreach (var condition in mainEventData.triggerConditions)
                {
                    if (!CheckCondition(condition))
                    {
                        isAllConditionCheck = false;
                        break;
                    }
                }

                if (isAllConditionCheck)
                {
                    validEvents.Add(mainEventData);
                }
            }

            if (validEvents.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, validEvents.Count);
            return validEvents[randomIndex];
        }


        private bool CheckCondition(Condition condition)
        {
            switch (condition.conditionType)
            {
                case EConditionType.CharacterInShelter:
                    {
                        return CharacterManager.IsInShelter(Enum.Parse<ECharacter>(condition.targetId));
                    }
                case EConditionType.AliveCountCheck:
                    {
                        int minValue = int.Parse(condition.value1);
                        int maxValue = int.Parse(condition.value2);
                        int aliveCount = CharacterManager.AliveCharacterCount();

                        return aliveCount >= minValue && aliveCount <= maxValue;
                    }
                case EConditionType.CharacterHasState:
                    {
                        var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(condition.targetId));
                        var state = EnumUtil.GetEnumByDescription<EState>(condition.parameter).Value;
                        return character.State.HasState(state);
                    }
                case EConditionType.CharacterNotHasState:
                    {
                        var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(condition.targetId));
                        var state = EnumUtil.GetEnumByDescription<EState>(condition.parameter).Value;
                        return !character.State.HasState(state);
                    }
                case EConditionType.CharacterExpeditionCountUpper:
                    {
                        var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(condition.targetId));
                        int countUpper = int.Parse(condition.value1);
                        return false;
                        //return CharacterManager.GetExpeditionCount(condition.targetId) > countUpper;
                    }
                case EConditionType.CharacterExpeditionCountLower:
                    {
                        int countLower = int.Parse(condition.value1);
                        return false;
                        //return CharacterManager.GetExpeditionCount(condition.targetId) < countLower;
                    }
                case EConditionType.HasItem:
                    {
                        return ItemManager.HasItem(Enum.Parse<EItem>(condition.targetId));
                    }
                case EConditionType.ItemCountUpper:
                    {
                        int itemUpper = int.Parse(condition.value1);
                        return ItemManager.GetItemCount(Enum.Parse<EItem>(condition.targetId)) >= itemUpper;
                    }
                case EConditionType.ItemCountLower:
                    {
                        int itemLower = int.Parse(condition.value1);
                        return ItemManager.GetItemCount(Enum.Parse<EItem>(condition.targetId)) <= itemLower;
                    }
                case EConditionType.DayCountUpper:
                    int minDay = int.Parse(condition.value1);
                    return GameManager.Instance.Day >= minDay;
                default:
                    Debug.LogWarning($"Unknown condition type: {condition.conditionType}");
                    return false;
            }
        }
    }
}
