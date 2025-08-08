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
    public interface IResultApplicator
    {
        EEffectType HandledEffectType { get; }

        void Apply(EventEffect effect);
    }

    /// <summary>
    /// 아이템 획득
    /// </summary>
    public class AddItemApplicator : IResultApplicator
    {
        public EEffectType HandledEffectType => EEffectType.AddItem;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public void Apply(EventEffect effect)
        {
            EItem item = Enum.Parse<EItem>(effect.targetId);
            float count = int.Parse(effect.value);
            ItemManager.AddItem(item, count);
            LogManager.AddRewardItemData(new RewardItemData(item, (int)count));
        }
    }

    /// <summary>
    /// 아이템 삭제
    /// </summary>
    public class RemoveItemApplicator : IResultApplicator
    {
        public EEffectType HandledEffectType => EEffectType.RemoveItem;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public void Apply(EventEffect effect)
        {
            EItem item = Enum.Parse<EItem>(effect.targetId);
            float count = int.Parse(effect.value);
            ItemManager.UsedItem(item, count);
            LogManager.AddRewardItemData(new RewardItemData(item, (int)-count));
        }
    }

    /// <summary>
    /// 엔딩 분기 진행
    /// </summary>
    public class AdvanceEndingProgressApplicator : IResultApplicator
    {
        public EEffectType HandledEffectType => EEffectType.AdvanceEndingProgress;

        public void Apply(EventEffect effect)
        {
            EEndingType endingType = Enum.Parse<EEndingType>(effect.targetId);
            EndingManager.Instance.AdvanceEndingProgress(endingType);
        }
    }

    /// <summary>
    /// 엔딩 완료
    /// </summary>
    public class EndingCompleteApplicator : IResultApplicator
    {
        public EEffectType HandledEffectType => EEffectType.EndingComplete;

        public void Apply(EventEffect effect)
        {
            EEndingType endingType = Enum.Parse<EEndingType>(effect.targetId);
            EndingManager.Instance.Ending(endingType);
        }
    }

    /// <summary>
    /// 스테이터스 악화
    /// </summary>
    public class WorsenStatusApplicator : IResultApplicator
    {
        public EEffectType HandledEffectType => EEffectType.WorsenStatus;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventEffect effect)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(effect.targetId));
            var status = Enum.Parse<EStatusType>(effect.parameter);
            var step = Mathf.Max(1, int.Parse(effect.value));

            character.Status.WorsenStatus(status, step);
        }
    }

    /// <summary>
    /// 스테이터스 치유
    /// </summary>
    public class RecoveryStatusApplicator : IResultApplicator
    {
        public EEffectType HandledEffectType => EEffectType.RecoveryStatus;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventEffect effect)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(effect.targetId));
            var status = Enum.Parse<EStatusType>(effect.parameter);
            var step = Mathf.Max(1, int.Parse(effect.value));

            character.Status.RecoveryStatus(status, step);
        }
    }

    /// <summary>
    /// 캐릭터 사망
    /// </summary>
    public class CharacterDaedApplicator : IResultApplicator
    {
        public EEffectType HandledEffectType => EEffectType.CharacterDaed;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventEffect effect)
        {
            var character = CharacterManager.GetCharacter(Enum.Parse<ECharacter>(effect.targetId));
            character.OnDead();
        }
    }
}