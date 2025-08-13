using System;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Expedition;
using WeWillSurvive.Item;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Util;

namespace WeWillSurvive
{
    public interface IEventConditionHandler
    {
        EConditionType HandledConditionType { get; }

        bool IsMet(Condition condition);
    }

    /// <summary>
    /// 캐릭터가 우주선 내에 존재하는지
    /// </summary>
    public class CharacterInShelterChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.CharacterInShelter;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(condition.targetId);
            var character = CharacterManager.GetCharacter(characterType);
            return character.IsInShelter;
        }
    }

    /// <summary>
    /// 캐릭터가 몇명 생존해 있는지
    /// </summary>
    public class AliveCountChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.AliveCount;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            if (!int.TryParse(condition.value1, out var minValue))
                Debug.LogWarning($"Value1 : {condition.value1} | int 타입으로 파싱 실패");

            if (!int.TryParse(condition.value2, out var maxValue))
                Debug.LogWarning($"Value2 : {condition.value1} | int 타입으로 파싱 실패");

            int aliveCount = CharacterManager.AliveCharactersCount();
            return aliveCount >= minValue && aliveCount <= maxValue;
        }
    }

    /// <summary>
    /// 캐릭터가 특정 상태(State)를 가지고 있는 지
    /// </summary>
    public class CharacterHasStateChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.CharacterHasState;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(condition.targetId);
            var character = CharacterManager.GetCharacter(characterType);

            EState state = EnumUtil.ParseEnum<EState>(condition.parameter);
            return character.State.HasState(state);
        }
    }

    /// <summary>
    /// 캐릭터가 특정 상태(State)를 가지고 있지 않은 지
    /// </summary>
    public class CharacterNotHasStateChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.CharacterNotHasState;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(condition.targetId);
            var character = CharacterManager.GetCharacter(characterType);

            EState state = EnumUtil.ParseEnum<EState>(condition.parameter);
            return !character.State.HasState(state);
        }
    }

    /// <summary>
    /// 총 탐사 횟수가 특정 값 이상 일시
    /// </summary>
    public class TotalExpeditionCountUpperChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.TotalExpeditionCountUpper;

        public bool IsMet(Condition condition)
        {
            if (!int.TryParse(condition.value1, out var countUpper))
                Debug.LogWarning($"Value1 : {condition.value1} | int 타입으로 파싱 실패");

            return ExpeditionManager.Instance.TotalExpeditionCount >= countUpper;
        }
    }

    /// <summary>
    /// 캐릭터의 탐사 횟수가 특정 값보다 높을 시
    /// </summary>
    public class CharacterExpeditionCountUpperChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.CharacterExpeditionCountUpper;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(condition.targetId);
            var character = CharacterManager.GetCharacter(characterType);

            if (!int.TryParse(condition.value1, out var countUpper))
                Debug.LogWarning($"Value1 : {condition.value1} | int 타입으로 파싱 실패");

            return character.TotalExploringCount > countUpper;
        }
    }

    /// <summary>
    /// 캐릭터의 탐사 횟수가 특정 값보다 낮을 시
    /// </summary>
    public class CharacterExpeditionCountLowerChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.CharacterExpeditionCountLower;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(condition.targetId);
            var character = CharacterManager.GetCharacter(characterType);

            if (!int.TryParse(condition.value1, out var countLower))
                Debug.LogWarning($"Value1 : {condition.value1} | int 타입으로 파싱 실패");

            return character.TotalExploringCount < countLower;
        }
    }

    /// <summary>
    /// 특정 아이템을 보유하고 있을 시
    /// </summary>
    public class HasItemChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.HasItem;
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public bool IsMet(Condition condition)
        {
            EItem item = EnumUtil.ParseEnum<EItem>(condition.targetId);
            return ItemManager.HasItem(item);
        }
    }

    /// <summary>
    /// 아이템 수량이 특정 값 이상일 시
    /// </summary>
    public class ItemCountUpperChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.ItemCountUpper;
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public bool IsMet(Condition condition)
        {
            EItem item = EnumUtil.ParseEnum<EItem>(condition.targetId);

            if (!int.TryParse(condition.value1, out var itemUpper))
                Debug.LogWarning($"Value1 : {condition.value1} | int 타입으로 파싱 실패");

            return ItemManager.GetItemCount(item) >= itemUpper;
        }
    }

    /// <summary>
    /// 아이템 수량이 특정 값 이하일 시
    /// </summary>
    public class ItemCountLowerChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.ItemCountLower;
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public bool IsMet(Condition condition)
        {
            EItem item = EnumUtil.ParseEnum<EItem>(condition.targetId);

            if (!int.TryParse(condition.value1, out var itemLower))
                Debug.LogWarning($"Value1 : {condition.value1} | int 타입으로 파싱 실패");

            return ItemManager.GetItemCount(item) <= itemLower;
        }
    }

    /// <summary>
    /// 특정 날짜 이후부터
    /// </summary>
    public class DayCountUpperChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.DayCountUpper;

        public bool IsMet(Condition condition)
        {
            if (!int.TryParse(condition.value1, out var day))
                Debug.LogWarning($"Value1 : {condition.value1} | int 타입으로 파싱 실패");

            return GameManager.Instance.Day >= day;
        }
    }
}
