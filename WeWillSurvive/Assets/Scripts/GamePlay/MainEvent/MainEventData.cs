using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace WeWillSurvive.MainEvent
{
    public enum EMainEventType
    {
        YesOrNo,            // "O" 또는 "X" 를 선택하는 이벤트
        UseItems,           // 특정 아이템을 사용하는 이벤트
        SendSomeone,        // 특정 대원을 탐사를 보내는 이벤트
        ChooseSomeon,       // 특정 대원을 선택하는 이벤트
        Trade               // 교환 이벤트
    }

    public enum EConditionType
    {
        [Description("캐릭터가 우주 기지에 있을 시")]
        CharacterInShelter,

        [Description("캐릭터의 상태가 특정 값보다 높을 시")]
        CharacterStatusUpper,

        [Description("캐릭터의 상태가 특정 값보다 낮을 시")]
        CharacterStatusLower,

        [Description("캐릭터가 특정 상태일 시")]
        CharacterHasState,

        [Description("캐릭터의 탐사 횟수가 특정 값보다 높을 시")]
        CharacterExpeditionCountUpper,

        [Description("캐릭터의 탐사 횟수가 특정 값보다 낮을 시")]
        CharacterExpeditionCountLower,

        [Description("특정 아이템을 보유하고 있을 시")]
        HasItem,

        [Description("아이템 수량이 특정 값 이상일 시")]
        ItemCountUpper,

        [Description("아이템 수량이 특정 값 이하일 시")]
        ItemCountLower,

        [Description("특정 날짜 이후부터")]
        DayCountUpper,
    }

    public enum EEffectType
    {
        AddItem,
        RemoveItem,
        ChangeStatus,
        InflictInjury,
        InflictSickness,
        Death,
        ChangeSanity,
        AdvanceEnding,
        AddFlag,
        RemoveFlag
    }

    [CreateAssetMenu(fileName = "MainEventData", menuName = "Scriptable Objects/MainEventData")]
    public class MainEventData : ScriptableObject
    {
        public string eventId;                                                              // 고유 ID (ex. "yesorno_01")
        public string title;                                                                // 엔딩 로그에 남길 타이틀

        [TextArea(3, 10)]
        public List<string> descriptions = new List<string>();                              // 랜덤 출력용 텍스트(이벤트 본문)
        public List<Condition> triggerConditions = new List<Condition>();                   // 이벤트 발생 조건
        public EMainEventType eventType;                                                    // 이벤트 타입 (YesOrNo, UseItem, ChooseSomeone 등)
        public List<EventChoice> choices;                                                   // 유저가 고를 수 있는 선택지
    }

    [System.Serializable]
    public class Condition
    {
        public EConditionType conditionType;
        public string targetId;   // 캐릭터 이름, 아이템 이름, 상태 이름 등
        public string parameter;  // 비교할 속성 (ex: Status, State 등)
        public string value;      // 비교할 값
    }

    [System.Serializable]
    public class EventChoice
    {
        public string choiceId;                 // 선택 ID
        public List<EventResult> results;       // 선택에 대한 결과 리스트
    }

    [System.Serializable]
    public class EventResult
    {
        public List<Condition> conditions;         // 해당 결과가 발생할 조건

        [TextArea(3, 10)]
        public string resultText;                  // 결과 텍스트

        public List<EventEffect> effects;          // 결과 보상

        [Range(0, 1)] 
        public float probability;                  // 발생 확률 (총합 1.0 안 넘게)
    }

    [System.Serializable]
    public class EventEffect
    {
        public EEffectType effectType;
        public string targetId;
        public string parameter;
        public string value;
    }
}
