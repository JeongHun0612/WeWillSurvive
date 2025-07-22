using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace WeWillSurvive.MainEvent
{
    #region Enum

    public enum EMainEventType
    {
        [Description("\"O\" 또는 \"X\" 를 선택하는 이벤트")]
        YesOrNo,            // "O" 또는 "X" 를 선택하는 이벤트

        [Description("특정 아이템을 사용하는 이벤트")]
        UseItems,           // 특정 아이템을 사용하는 이벤트

        [Description("특정 대원을 탐사를 보내는 이벤트")]
        SendSomeone,        // 특정 대원을 탐사를 보내는 이벤트

        [Description("특정 대원을 선택하는 이벤트")]
        ChooseSomeone,      // 특정 대원을 선택하는 이벤트

        [Description("교환 이벤트")]
        Trade,              // 교환 이벤트

        [Description("이벤트가 존재하지 않을 시")]
        Noting,
    }

    public enum EConditionType
    {
        [InspectorName("캐릭터가 우주 기지에 있을 시")]
        CharacterInShelter,

        [InspectorName("생존 인원이 특정 값 사이일 때")]
        [Tooltip("Value1(최소) ~ Value2(최대) 까지의 인원")]
        AliveCountCheck,

        [InspectorName("캐릭터가 특정 상태일 시")]
        CharacterHasState,

        [InspectorName("캐릭터가 특정 상태가 아닐 시")]
        CharacterNotHasState,

        [InspectorName("캐릭터의 탐사 횟수가 특정 값보다 높을 시")]
        CharacterExpeditionCountUpper,

        [InspectorName("캐릭터의 탐사 횟수가 특정 값보다 낮을 시")]
        CharacterExpeditionCountLower,

        [InspectorName("특정 아이템을 보유하고 있을 시")]
        HasItem,

        [InspectorName("아이템 수량이 특정 값 이상일 시")]
        ItemCountUpper,

        [InspectorName("아이템 수량이 특정 값 이하일 시")]
        ItemCountLower,

        [InspectorName("특정 날짜 이후부터")]
        DayCountUpper,
    }

    public enum EChoiceType
    {
        // O, X 아이콘
        //[InspectorName("...")]
        [Description("Yes")] Yes = 0,
        [Description("No")] No,

        // 캐릭터 아이콘
        [Description("Lead")] Lead = 100,
        [Description("Cook")] Cook,
        [Description("Bell")] Bell,
        [Description("DrK")] DrK,

        // 아이템 아이콘
        [Description("우주식량")] Food = 200,
        [Description("특별우주식량")] SpecialFood,
        [Description("물")] Water,
        [Description("의료키트")] MedicKit,
        [Description("특별의료키트")] SpecialMedicKit,
        [Description("수리키트")] RepairKit,
        [Description("특별수리키트")] SpecialRepairKit,
        [Description("예비통신장비")] CommDevice,
        [Description("고급우주복")] NiceSpacesuit,
        [Description("총")] Gun,
        [Description("보드게임")] BoardGame,
        [Description("도끼")] Ax,
        [Description("쇠파이프")] Pipe,
        [Description("손전등")] Flashlight,
        [Description("행성탐사지도")] Map,

        // 기타
        [Description("아무것도 선택하지 않음")] Noting = 300,
        [Description("누군가를 보냄")] Sendsomeone,

        None,
    }

    public enum EEffectType
    {
        [Description("아이템 획득")] AddItem,
        [Description("아이템 삭제")] RemoveItem,

        [Description("스테이터스 증가")] IncreaseStatus,
        [Description("스테이터스 감소")] DecreaseStatus,
    }
    #endregion

    [CreateAssetMenu(fileName = "MainEventData", menuName = "Scriptable Objects/MainEventData")]
    public class MainEventData : ScriptableObject
    {
        public string eventId;                                                              // 고유 ID (ex. "yesorno_01")
        public string title;                                                                // 엔딩 로그에 남길 타이틀

        [TextArea(3, 10)]
        public List<string> descriptions = new();                                           // 랜덤 출력용 텍스트(이벤트 본문)
        public List<Condition> triggerConditions = new();                                   // 이벤트 발생 조건
        public EMainEventType eventType;                                                    // 이벤트 타입 (YesOrNo, UseItem, ChooseSomeone 등)
        public List<EventChoice> choices;                                                   // 유저가 고를 수 있는 선택지

        public EventChoice GetEventChoice(EChoiceType choiceType) => choices.FirstOrDefault(choice => choice.choiceType == choiceType);
    }

    [System.Serializable]
    public class Condition
    {
        public EConditionType conditionType;
        public string targetId;   // 캐릭터 이름, 아이템 이름, 상태 이름 등
        public string parameter;  // 비교할 속성 (ex: Status, State 등)
        public string value1;     // 비교할 값 1
        public string value2;     // 비교할 값 2
    }

    [System.Serializable]
    public class EventChoice
    {
        public EChoiceType choiceType;          // 선택 ID
        public List<EventResult> results;       // 선택에 대한 결과 리스트

        public EventResult GetRandomResult()
        {
            if (results == null || results.Count == 0)
                return null;

            int index = Random.Range(0, results.Count);
            return results[index];
        }
    }

    [System.Serializable]
    public class EventResult
    {
        public List<Condition> conditions;      // 해당 결과가 발생할 조건

        [TextArea(3, 10)]
        public string resultText;               // 결과 텍스트

        public List<EventEffect> effects;       // 결과 보상

        [Range(0, 1)] 
        public float probability;               // 발생 확률 (총합 1.0 안 넘게)

        public EventResult()
        {
            conditions = new();
            resultText = string.Empty;
            effects = new();
            probability = 1.0f;
        }
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
