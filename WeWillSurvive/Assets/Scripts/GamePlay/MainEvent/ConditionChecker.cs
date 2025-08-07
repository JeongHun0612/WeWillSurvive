using System;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Status;
using WeWillSurvive.Util;

namespace WeWillSurvive
{
    public interface IConditionChecker
    {
        EConditionType HandledConditionType { get; }

        bool IsMet(Condition condition);
    }

    /// <summary>
    /// 캐릭터가 우주선 내에 존재하는지
    /// </summary>
    public class CharacterInShelterChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.CharacterInShelter;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(condition.targetId));
            return character.IsInShelter;
        }
    }

    /// <summary>
    /// 캐릭터가 몇명 생존해 있는지
    /// </summary>
    public class AliveCountChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.AliveCount;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            int minValue = int.Parse(condition.value1);
            int maxValue = int.Parse(condition.value2);
            int aliveCount = CharacterManager.AliveCharacterCount();

            return aliveCount >= minValue && aliveCount <= maxValue;
        }
    }

    /// <summary>
    /// 캐릭터가 특정 상태(State)를 가지고 있는 지
    /// </summary>
    public class CharacterHasStateChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.CharacterHasState;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(condition.targetId));
            var state = EnumUtil.GetEnumByDescription<EState>(condition.parameter).Value;
            return character.State.HasState(state);
        }
    }

    /// <summary>
    /// 캐릭터가 특정 상태(State)를 가지고 있지 않은 지
    /// </summary>
    public class CharacterNotHasStateChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.CharacterNotHasState;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(condition.targetId));
            var state = EnumUtil.GetEnumByDescription<EState>(condition.parameter).Value;
            return !character.State.HasState(state);
        }
    }

    /// <summary>
    /// 캐릭터가 특정 상태(Status)를 가지고 있는 지
    /// </summary>
    public class CharacterHasStatusChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.CharacterHasStatus;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(condition.targetId));
            var status = EnumUtil.GetEnumByDescription<EStatusType>(condition.parameter).Value;
            return character.Status.HasStatus(status);
        }
    }

    /// <summary>
    /// 캐릭터가 특정 상태(Status)를 가지고 있지 않은 지
    /// </summary>
    public class CharacterNotHasStatusChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.CharacterNotHasStatus;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(condition.targetId));
            var status = EnumUtil.GetEnumByDescription<EStatusType>(condition.parameter).Value;
            return !character.Status.HasStatus(status);
        }
    }

    /// <summary>
    /// 캐릭터의 탐사 횟수가 특정 값보다 높을 시
    /// </summary>
    public class CharacterExpeditionCountUpperChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.CharacterExpeditionCountUpper;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(condition.targetId));
            int countUpper = int.Parse(condition.value1);
            return character.TotalExploringCount > countUpper;
        }
    }

    /// <summary>
    /// 캐릭터의 탐사 횟수가 특정 값보다 낮을 시
    /// </summary>
    public class CharacterExpeditionCountLowerChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.CharacterExpeditionCountLower;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public bool IsMet(Condition condition)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(condition.targetId));
            int countLower = int.Parse(condition.value1);
            return character.TotalExploringCount < countLower;
        }
    }

    /// <summary>
    /// 특정 아이템을 보유하고 있을 시
    /// </summary>
    public class HasItemChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.HasItem;
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public bool IsMet(Condition condition)
        {
            return ItemManager.HasItem(Enum.Parse<EItem>(condition.targetId));
        }
    }

    /// <summary>
    /// 아이템 수량이 특정 값 이상일 시
    /// </summary>
    public class ItemCountUpperChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.ItemCountUpper;
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public bool IsMet(Condition condition)
        {
            int itemUpper = int.Parse(condition.value1);
            return ItemManager.GetItemCount(Enum.Parse<EItem>(condition.targetId)) >= itemUpper;
        }
    }

    /// <summary>
    /// 아이템 수량이 특정 값 이하일 시
    /// </summary>
    public class ItemCountLowerChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.ItemCountLower;
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public bool IsMet(Condition condition)
        {
            int itemLower = int.Parse(condition.value1);
            return ItemManager.GetItemCount(Enum.Parse<EItem>(condition.targetId)) <= itemLower;
        }
    }

    /// <summary>
    /// 특정 날짜 이후부터
    /// </summary>
    public class DayCountUpperChecker : IConditionChecker
    {
        public EConditionType HandledConditionType => EConditionType.DayCountUpper;

        public bool IsMet(Condition condition)
        {
            int minDay = int.Parse(condition.value1);
            return GameManager.Instance.Day >= minDay;
        }
    }
}
