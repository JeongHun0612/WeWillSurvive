using System;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.Log;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Status;

namespace WeWillSurvive
{
    public interface IEventActionHandler
    {
        EActionType HandledActionType { get; }

        void Apply(EventAction action);
    }

    /// <summary>
    /// 아이템 획득
    /// </summary>
    public class AddItemHandler : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.AddItem;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public void Apply(EventAction action)
        {
            EItem item = Enum.Parse<EItem>(action.targetId);
            float count = int.Parse(action.value);
            ItemManager.AddItem(item, count);
            LogManager.AddRewardItemData(new RewardItemData(item, (int)count));
        }
    }

    /// <summary>
    /// 아이템 삭제
    /// </summary>
    public class RemoveItemApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.RemoveItem;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public void Apply(EventAction action)
        {
            EItem item = Enum.Parse<EItem>(action.targetId);
            float count = int.Parse(action.value);
            ItemManager.UsedItem(item, count);
            LogManager.AddRewardItemData(new RewardItemData(item, (int)-count));
        }
    }

    /// <summary>
    /// 엔딩 분기 진행
    /// </summary>
    public class AdvanceEndingProgressApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.AdvanceEndingProgress;

        public void Apply(EventAction action)
        {
            EEndingType endingType = Enum.Parse<EEndingType>(action.targetId);
            EndingManager.Instance.AdvanceEndingProgress(endingType);
        }
    }

    /// <summary>
    /// 엔딩 완료
    /// </summary>
    public class EndingCompleteApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.EndingComplete;

        public void Apply(EventAction action)
        {
            EEndingType endingType = Enum.Parse<EEndingType>(action.targetId);
            EndingManager.Instance.Ending(endingType);
        }
    }

    /// <summary>
    /// 스테이터스 악화
    /// </summary>
    public class WorsenStatusApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.WorsenStatus;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(action.targetId));
            var status = Enum.Parse<EStatusType>(action.parameter);
            var step = Mathf.Max(1, int.Parse(action.value));

            character.Status.WorsenStatus(status, step);
        }
    }

    /// <summary>
    /// 스테이터스 치유
    /// </summary>
    public class RecoveryStatusApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.RecoveryStatus;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(action.targetId));
            var status = Enum.Parse<EStatusType>(action.parameter);
            var step = Mathf.Max(1, int.Parse(action.value));

            character.Status.RecoveryStatus(status, step);
        }
    }

    /// <summary>
    /// 캐릭터 사망
    /// </summary>
    public class CharacterDaedApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.CharacterDaed;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(action.targetId));
            character.OnDead();
        }
    }
}