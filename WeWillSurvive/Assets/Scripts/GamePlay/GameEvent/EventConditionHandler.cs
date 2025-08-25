using System;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Expedition;
using WeWillSurvive.Item;
using WeWillSurvive.Util;

namespace WeWillSurvive.GameEvent
{
    #region EConditionType
    public enum EConditionType
    {
        /// 캐릭터
        [InspectorName("캐릭터가 우주 기지에 있을 시")]
        CharacterInShelter = 100,

        [InspectorName("캐릭터가 특정 상태(State)를 보유하고 있을 시")]
        CharacterHasState = 101,

        [InspectorName("캐릭터가 특정 상태(State)를 보유하고 있지 않을 시")]
        CharacterNotHasState = 102,

        [InspectorName("캐릭터의 탐사 횟수가 특정 값보다 높을 시")]
        CharacterExpeditionCountUpper = 103,

        [InspectorName("캐릭터의 탐사 횟수가 특정 값보다 낮을 시")]
        CharacterExpeditionCountLower = 104,

        /// 아이템
        [InspectorName("특정 아이템을 보유하고 있을 시")]
        HasItem = 200,
        [InspectorName("특정 아이템을 보유하고 있지 않을 시")]
        NotHasItem = 201,

        [InspectorName("아이템 수량이 특정 값 이상일 시")]
        ItemCountUpper = 202,

        [InspectorName("아이템 수량이 특정 값 이하일 시")]
        ItemCountLower = 203,

        // 버프
        [InspectorName("특정 버프가 있을 시")]
        HasBuffEffect = 300,
        [InspectorName("특정 버프가 없을 시")]
        NotHasBuffEffect = 301,

        /// 기타
        [InspectorName("생존 인원이 특정 값 사이일 때")]
        [Tooltip("Value1(최소) ~ Value2(최대) 까지의 인원")]
        AliveCount = 1000,

        [InspectorName("총 탐사 횟수가 특정 값 이상 일시")]
        TotalExpeditionCountUpper = 1001,

        [InspectorName("특정 날짜 이후부터")]
        DayCountUpper = 1002,
    }
    #endregion

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
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(condition.TargetId);
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
            if (!int.TryParse(condition.Value1, out var minValue))
                Debug.LogWarning($"Value1 : {condition.Value1} | int 타입으로 파싱 실패");

            if (!int.TryParse(condition.Value2, out var maxValue))
                Debug.LogWarning($"Value2 : {condition.Value2} | int 타입으로 파싱 실패");

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
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(condition.TargetId);
            var character = CharacterManager.GetCharacter(characterType);

            EState state = EnumUtil.ParseEnum<EState>(condition.Parameter);
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
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(condition.TargetId);
            var character = CharacterManager.GetCharacter(characterType);

            EState state = EnumUtil.ParseEnum<EState>(condition.Parameter);
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
            if (!int.TryParse(condition.Value1, out var countUpper))
                Debug.LogWarning($"Value1 : {condition.Value1} | int 타입으로 파싱 실패");

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
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(condition.TargetId);
            var character = CharacterManager.GetCharacter(characterType);

            if (!int.TryParse(condition.Value1, out var countUpper))
                Debug.LogWarning($"Value1 : {condition.Value1} | int 타입으로 파싱 실패");

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
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(condition.TargetId);
            var character = CharacterManager.GetCharacter(characterType);

            if (!int.TryParse(condition.Value1, out var countLower))
                Debug.LogWarning($"Value1 : {condition.Value1} | int 타입으로 파싱 실패");

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
            EItem item = EnumUtil.ParseEnum<EItem>(condition.TargetId);
            return ItemManager.HasItem(item);
        }
    }

    /// <summary>
    /// 특정 아이템을 보유하고 있지 않을 시
    /// </summary>
    public class NotHasItemChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.NotHasItem;
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public bool IsMet(Condition condition)
        {
            EItem item = EnumUtil.ParseEnum<EItem>(condition.TargetId);
            return !ItemManager.HasItem(item);
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
            EItem item = EnumUtil.ParseEnum<EItem>(condition.TargetId);

            if (!int.TryParse(condition.Value1, out var itemUpper))
                Debug.LogWarning($"Value1 : {condition.Value1} | int 타입으로 파싱 실패");

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
            EItem item = EnumUtil.ParseEnum<EItem>(condition.TargetId);

            if (!int.TryParse(condition.Value1, out var itemLower))
                Debug.LogWarning($"Value1 : {condition.Value1} | int 타입으로 파싱 실패");

            return ItemManager.GetItemCount(item) <= itemLower;
        }
    }

    /// <summary>
    /// 특정 버프가 있을 시
    /// </summary>
    public class HasBuffEffectChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.HasBuffEffect;

        public bool IsMet(Condition condition)
        {
            EBuffEffect effect = EnumUtil.ParseEnum<EBuffEffect>(condition.TargetId);
            return BuffManager.Instance.HasBuff(effect);
        }
    }

    /// <summary>
    /// 특정 버프가 없을 시
    /// </summary>
    public class NotHasBuffEffectChecker : IEventConditionHandler
    {
        public EConditionType HandledConditionType => EConditionType.NotHasBuffEffect;

        public bool IsMet(Condition condition)
        {
            EBuffEffect effect = EnumUtil.ParseEnum<EBuffEffect>(condition.TargetId);
            return !BuffManager.Instance.HasBuff(effect);
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
            if (!int.TryParse(condition.Value1, out var day))
                Debug.LogWarning($"Value1 : {condition.Value1} | int 타입으로 파싱 실패");

            return GameManager.Instance.Day >= day;
        }
    }
}
