using System;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.Log;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Status;
using WeWillSurvive.Util;

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
            EItem item = EnumUtil.ParseEnum<EItem>(action.targetId);

            if (!int.TryParse(action.value, out var count))
                Debug.LogWarning($"Value : {action.value} | int 타입으로 파싱 실패");

            ItemManager.AddItem(item, count);
            LogManager.AddRewardItemData(new RewardItemData(item, count));
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
            EItem item = EnumUtil.ParseEnum<EItem>(action.targetId);

            if (!int.TryParse(action.value, out var count))
                Debug.LogWarning($"Value : {action.value} | int 타입으로 파싱 실패");

            if (ItemManager.TryDecreaseItemCount(item, count))
            {
                LogManager.AddRewardItemData(new RewardItemData(item, -count));
            }
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
            EEndingType endingType = EnumUtil.ParseEnum<EEndingType>(action.targetId);
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
            EEndingType endingType = EnumUtil.ParseEnum<EEndingType>(action.targetId);
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
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(action.targetId);
            var character = CharacterManager.GetCharacter(characterType);
            var status = EnumUtil.ParseEnum<EStatusType>(action.parameter);

            if (!int.TryParse(action.value, out var value))
                Debug.LogWarning($"Value : {action.value} | int 타입으로 파싱 실패");

            var step = Mathf.Max(1, value);

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
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(action.targetId);
            var character = CharacterManager.GetCharacter(characterType);
            var status = EnumUtil.ParseEnum<EStatusType>(action.parameter);

            if (!int.TryParse(action.value, out var value))
                Debug.LogWarning($"Value : {action.value} | int 타입으로 파싱 실패");

            var step = Mathf.Max(1, value);

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
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(action.targetId);
            var character = CharacterManager.GetCharacter(characterType);
            character.OnDead();
        }
    }
}