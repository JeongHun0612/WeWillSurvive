using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Ending;
using WeWillSurvive.Item;
using WeWillSurvive.Log;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Status;
using WeWillSurvive.Util;

namespace WeWillSurvive.GameEvent
{
    public enum EActionType
    {
        [InspectorName("스테이터스 악화")] WorsenStatus = 100,
        [InspectorName("스테이터스 최대 악화")] WorsenMaxStatus = 105,

        [InspectorName("스테이터스 치유")] RecoveryStatus = 101,
        [InspectorName("스테이터스 완전 치유")] RecoveryMaxStatus = 106,

        [InspectorName("캐릭터 사망")] CharacterDaed = 102,
        [InspectorName("캐릭터 이벤트 확률 보정")] CharacterEventRateModifier = 103,

        [InspectorName("랜덤한 캐릭터 상태 악화 (완전 랜덤)")] WorsenRandomCharacterStatus = 110,
        [InspectorName("랜덤한 캐릭터 상태 악화 (정상 우선)")] WorsenPrioritizedCharacterStatus = 111,

        [InspectorName("아이템 획득")] AddItem = 200,
        [InspectorName("아이템 삭제")] RemoveItem = 201,
        [InspectorName("식량, 물을 제외한 랜덤한 아이템 삭제")] RemoveRandomSupportItem = 202,
        [InspectorName("물과 식량 중 더 많이 소지한 아이템 삭제")] RemoveGreaterOfFoodAndWater = 203,

        [InspectorName("엔딩 분기 진행")] AdvanceEndingProgress = 300,
        [InspectorName("엔딩 완료")] EndingComplete = 301,

        [InspectorName("특정 메인 이벤트 쿨타임 설정")] SetSpecificMainEventCooldown = 400,
        [InspectorName("특정 메인 이벤트 쿨타임 추가")] AddDaysToSpecificMainEventCooldown = 401,

        [InspectorName("특정 버프 발생")] ActivateBuff = 500,
        [InspectorName("다음 캐릭터 이벤트까지 특정 버프 발생")] ActivateBuffUntilNextCharacterEvent = 501,

        [InspectorName("침입 이벤트 방어 성공")] InvasionDefenseSuccess = 600,
        [InspectorName("침입 이벤트 방어 실패")] InvasionDefenseFailure = 601,
    }

    public interface IEventActionHandler
    {
        EActionType HandledActionType { get; }

        void Apply(EventAction action, ref string finalResultText, IReadOnlyList<string> resultTemplates = null);
    }

    /// <summary>
    /// 스테이터스 악화
    /// </summary>
    public class WorsenStatusApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.WorsenStatus;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(action.TargetId);
            var character = CharacterManager.GetCharacter(characterType);
            var statusType = EnumUtil.ParseEnum<EStatusType>(action.Parameter);

            if (!int.TryParse(action.Value, out var Value))
                Debug.LogWarning($"Value : {action.Value} | int 타입으로 파싱 실패");

            var step = Mathf.Max(1, Value);

            var status = character.Status.GetStatus<IStatus>(statusType);
            if (status != null)
            {
                status.WorsenStatus(step);
            }
        }
    }

    /// <summary>
    /// 스테이터스 최대 악화
    /// </summary>
    public class WorsenMaxStatusApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.WorsenMaxStatus;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(action.TargetId);
            var character = CharacterManager.GetCharacter(characterType);
            var statusType = EnumUtil.ParseEnum<EStatusType>(action.Parameter);
            var status = character.Status.GetStatus<IStatus>(statusType);
            if (status != null)
            {
                status.WorsenFully();
            }
        }
    }

    /// <summary>
    /// 스테이터스 치유
    /// </summary>
    public class RecoveryStatusApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.RecoveryStatus;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(action.TargetId);
            var character = CharacterManager.GetCharacter(characterType);
            var statusType = EnumUtil.ParseEnum<EStatusType>(action.Parameter);

            if (!int.TryParse(action.Value, out var Value))
                Debug.LogWarning($"Value : {action.Value} | int 타입으로 파싱 실패");

            var step = Mathf.Max(1, Value);

            var status = character.Status.GetStatus<IStatus>(statusType);
            if (status != null)
            {
                status.RecoveryStatus(step);
            }
        }
    }

    /// <summary>
    /// 스테이터스 완전 치유
    /// </summary>
    public class RecoveryMaxStatusApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.RecoveryMaxStatus;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(action.TargetId);
            var character = CharacterManager.GetCharacter(characterType);
            var statusType = EnumUtil.ParseEnum<EStatusType>(action.Parameter);
            var status = character.Status.GetStatus<IStatus>(statusType);
            if (status != null)
            {
                status.RecoverFully();
            }
        }
    }

    /// <summary>
    /// 캐릭터 사망
    /// </summary>
    public class CharacterDaedApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.CharacterDaed;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(action.TargetId);
            var character = CharacterManager.GetCharacter(characterType);
            character.OnDead();
        }
    }

    /// <summary>
    /// 캐릭터 이벤트 확률 보정
    /// </summary>
    public class CharacterEventRateModifierApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.CharacterEventRateModifier;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            ECharacter characterType = EnumUtil.ParseEnum<ECharacter>(action.TargetId);
            var character = CharacterManager.GetCharacter(characterType);

            if (!float.TryParse(action.Value, out var modifier))
                Debug.LogWarning($"Value : {action.Value} | float 타입으로 파싱 실패");

            character.EventSelectionModifier = modifier;
        }
    }


    /// <summary>
    /// 랜덤한 캐릭터 상태 악화 (완전 랜덤)
    /// </summary>
    public class WorsenRandomCharacterStatusApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.WorsenRandomCharacterStatus;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            var statusType = EnumUtil.ParseEnum<EStatusType>(action.Parameter);

            var characters = CharacterManager.GetCharactersInShelter();
            int randomIndex = UnityEngine.Random.Range(0, characters.Count);
            var targetCharacter = characters[randomIndex];

            var status = targetCharacter.Status.GetStatus<IStatus>(statusType);
            status.WorsenStatus();
            
            if (resultTemplates != null && resultTemplates.Count > 0)
            {
                resultText = resultTemplates[0].Replace("{}", targetCharacter.Name);
            }
        }
    }

    /// <summary>
    /// 랜덤한 캐릭터 상태 악화 (정상 우선)
    /// </summary>
    public class WorsenPrioritizedCharacterStatusApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.WorsenPrioritizedCharacterStatus;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            var statusType = EnumUtil.ParseEnum<EStatusType>(action.Parameter);

            var targetCharacters = CharacterManager.GetCharactersInShelter()
                .Select(character => new
                {
                    Character = character,
                    Level = character.Status.GetStatus<IStatus>(statusType).GetCurrentLevel()
                })
                .GroupBy(x => x.Level)
                .OrderBy(g => g.Key)
                .First()
                .Select(x => x.Character)
                .ToList();

            int randomIndex = UnityEngine.Random.Range(0, targetCharacters.Count);
            var targetCharacter = targetCharacters[randomIndex];
            var status = targetCharacter.Status.GetStatus<IStatus>(statusType);
            status.WorsenStatus();

            if (resultTemplates != null && resultTemplates.Count > 0)
            {
                resultText = resultTemplates[0].Replace("{}", targetCharacter.Name);
            }
        }
    }

    /// <summary>
    /// 아이템 획득
    /// </summary>
    public class AddItemHandler : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.AddItem;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            EItem item = EnumUtil.ParseEnum<EItem>(action.TargetId);

            if (!float.TryParse(action.Value, out var count))
                Debug.LogWarning($"Value : {action.Value} | float 타입으로 파싱 실패");

            ItemManager.AddItem(item, count);
            LogManager.AddResultItemData(new ResultItemData(item, count));
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

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            EItem item = EnumUtil.ParseEnum<EItem>(action.TargetId);

            if (!float.TryParse(action.Value, out var count))
                Debug.LogWarning($"Value : {action.Value} | float 타입으로 파싱 실패");

            var updateCount = Mathf.Min(ItemManager.GetItemCount(item), count);
            if (ItemManager.TryDecreaseItemCount(item, updateCount))
            {
                LogManager.AddResultItemData(new ResultItemData(item, -updateCount));
            }
        }
    }

    /// <summary>
    /// 식량, 물을 제외한 랜덤한 아이템 삭제
    /// </summary>
    public class RemoveRandomSupportItemApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.RemoveRandomSupportItem;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            EItem item = ItemManager.GetRandomSupportItem();
            if (item == EItem.None)
            {
                resultText = resultTemplates[1];
                return;
            }

            resultText = resultTemplates[0].Replace("{}", EnumUtil.GetInspectorName(item));
            var updateCount = Mathf.Min(ItemManager.GetItemCount(item), 1f);
            if (ItemManager.TryDecreaseItemCount(item, updateCount))
            {
                LogManager.AddResultItemData(new ResultItemData(item, -updateCount));
            }
        }
    }

    /// <summary>
    /// 물과 식량 중 더 많이 소지한 아이템 삭제
    /// </summary>
    public class RemoveGreaterOfFoodAndWaterApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.RemoveGreaterOfFoodAndWater;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();
        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            if (!float.TryParse(action.Value, out var removeCount))
                Debug.LogWarning($"Value : {action.Value} | float 타입으로 파싱 실패");

            var foodCount = ItemManager.GetItemCount(EItem.Food);
            var waterCount = ItemManager.GetItemCount(EItem.Water);

            if (foodCount == 0f && waterCount == 0f)
            {
                resultText = resultTemplates[1];
                return;
            }

            EItem removeItem = (foodCount == waterCount)
                ? (UnityEngine.Random.value < 0.5f ? EItem.Food : EItem.Water)
                : (foodCount > waterCount ? EItem.Food : EItem.Water);

            resultText = resultTemplates[0].Replace("{}", EnumUtil.GetInspectorName(removeItem));
            var updateCount = Mathf.Min(ItemManager.GetItemCount(removeItem), removeCount);
            if (ItemManager.TryDecreaseItemCount(removeItem, updateCount))
            {
                LogManager.AddResultItemData(new ResultItemData(removeItem, -updateCount));
            }
        }
    }

    /// <summary>
    /// 엔딩 분기 진행
    /// </summary>
    public class AdvanceEndingProgressApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.AdvanceEndingProgress;

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            EEndingType endingType = EnumUtil.ParseEnum<EEndingType>(action.TargetId);

            string logText;
            if (resultTemplates == null || resultTemplates.Count == 0)
                logText = $"{GameManager.Instance.Day}일차";
            else
                logText = resultTemplates[0].Replace("{}", GameManager.Instance.Day.ToString());

            GameEventManager.Instance.EndingEventPicker.AdvanceEndingProgress(endingType, logText);
        }
    }

    /// <summary>
    /// 엔딩 완료
    /// </summary>
    public class EndingCompleteApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.EndingComplete;

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            EEndingType endingType = EnumUtil.ParseEnum<EEndingType>(action.TargetId);

            var eventProgress = GameEventManager.Instance.EndingEventPicker.GetEventProgress(endingType);
            resultText = resultText + "\n\n" + eventProgress.GetResultLogText();

            EndingManager.Instance.Ending(endingType);
        }
    }

    /// <summary>
    /// 특정 메인 이벤트 쿨타임 설정
    /// </summary>
    public class SetSpecificMainEventCooldownApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.SetSpecificMainEventCooldown;
        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            EMainEventCategory category = EnumUtil.ParseEnum<EMainEventCategory>(action.TargetId);

            if (!int.TryParse(action.Value, out var dayCounter))
                Debug.LogWarning($"Value : {action.Value} | int 타입으로 파싱 실패");

            var targetEventProgress = GameEventManager.Instance.MainEventPicker.GetEventProgress(category);

            targetEventProgress?.SetDayCounter(dayCounter);
        }
    }

    /// <summary>
    /// 특정 메인 이벤트 쿨타임 추가
    /// </summary>
    public class AddDaysToSpecificMainEventCooldownApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.AddDaysToSpecificMainEventCooldown;
        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            EMainEventCategory category = EnumUtil.ParseEnum<EMainEventCategory>(action.TargetId);

            if (!int.TryParse(action.Value, out var dayCounter))
                Debug.LogWarning($"Value : {action.Value} | int 타입으로 파싱 실패");

            var targetEventProgress = GameEventManager.Instance.MainEventPicker.GetEventProgress(category);

            targetEventProgress?.AddDayCounter(dayCounter);
        }
    }

    /// <summary>
    /// 특정 버프 발생
    /// </summary>
    public class ActivateBuffApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.ActivateBuff;
        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            EBuffEffect effect = EnumUtil.ParseEnum<EBuffEffect>(action.TargetId);

            if (!int.TryParse(action.Value, out var duration))
                Debug.LogWarning($"Value : {action.Value} | int 타입으로 파싱 실패");

            BuffManager.Instance.AddBuff(effect, duration);
        }
    }

    /// <summary>
    /// 다음 캐릭터 이벤트까지 특정 버프 발생
    /// </summary>
    public class ActivateBuffUntilNextCharacterEventApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.ActivateBuffUntilNextCharacterEvent;
        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            EBuffEffect effect = EnumUtil.ParseEnum<EBuffEffect>(action.TargetId);
            var duration = GameEventManager.Instance.CharacterEventPicker.GlobalDayCounter;

            BuffManager.Instance.AddBuff(effect, duration);
        }
    }

    /// <summary>
    /// 침입 이벤트 방어 성공
    /// </summary>
    public class InvasionDefenseSuccessApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.InvasionDefenseSuccess;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            var characters = CharacterManager.GetCharactersInShelter();
            int randomIndex = UnityEngine.Random.Range(0, characters.Count);
            var targetCharacter = characters[randomIndex];
            resultText = resultText.Replace("{}", targetCharacter.Name);
        }
    }

    /// <summary>
    /// 침입 이벤트 방어 실패
    /// </summary>
    public class InvasionDefenseFailureApplicator : IEventActionHandler
    {
        public EActionType HandledActionType => EActionType.InvasionDefenseFailure;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(EventAction action, ref string resultText, IReadOnlyList<string> resultTemplates = null)
        {
            var characters = CharacterManager.GetCharactersInShelter();
            int randomIndex = UnityEngine.Random.Range(0, characters.Count);
            var targetCharacter = characters[randomIndex];

            var status = targetCharacter.Status.GetStatus<InjuryStatus>(EStatusType.Injury);
            if (status.Level == EInjuredLevel.Normal)
            {
                status.WorsenStatus();
                resultText = resultTemplates[0].Replace("{}", targetCharacter.Name);
            }
            else if (status.Level == EInjuredLevel.Injured || status.Level == EInjuredLevel.Sick)
            {
                string deadMessage = string.Format(resultTemplates[2], GameManager.Instance.Day, targetCharacter.Name);
                targetCharacter.OnDead(deadMessage);
                resultText = resultTemplates[1].Replace("{}", targetCharacter.Name);
            }
        }
    }
}