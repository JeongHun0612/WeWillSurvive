using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace WeWillSurvive.MainEvent
{
    #region Enum
    public enum EMainEventChoiceSchema
    {
        [InspectorName("\"Yes\" 또는 \"No\" 중 하나를 선택하는 이벤트")]
        YesOrNo = 0,

        [InspectorName("특정 아이템 중 하나를 선택하는 이벤트")]
        UseItems = 1,

        [InspectorName("모든 대원 중 한명을 선택하는 이벤트")]
        SendSomeone = 2,

        [InspectorName("특정 대원 중 한명을 선택하는 이벤트")]
        ChooseSomeone = 3,

        [InspectorName("외부 친입에 무기(도끼, 총, 쇠파이프)를 사용해서 방어하는 이벤트")]
        Invasion = 4,

        [InspectorName("조사(손전등, 맨손) 이벤트")]
        Exploration = 5,

        [InspectorName("선택지 없음")]
        Noting = 100,
    }

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

        [InspectorName("아이템 수량이 특정 값 이상일 시")]
        ItemCountUpper = 201,

        [InspectorName("아이템 수량이 특정 값 이하일 시")]
        ItemCountLower = 202,


        /// 기타
        [InspectorName("생존 인원이 특정 값 사이일 때")]
        [Tooltip("Value1(최소) ~ Value2(최대) 까지의 인원")]
        AliveCount = 1000,

        [InspectorName("총 탐사 횟수가 특정 값 이상 일시")]
        TotalExpeditionCountUpper = 1001,

        [InspectorName("특정 날짜 이후부터")]
        DayCountUpper = 1002,
    }

    public enum EChoiceIcon
    {
        // O, X 아이콘
        [InspectorName("Yes")] Yes = 0,
        [InspectorName("No")] No = 1,

        // 캐릭터 아이콘
        [InspectorName("Lead")] Lead = 100,
        [InspectorName("Cook")] Cook = 101,
        [InspectorName("Bell")] Bell = 102,
        [InspectorName("DrK")] DrK = 103,

        // 아이템 아이콘
        [InspectorName("우주식량")] Food = 200,
        [InspectorName("특별우주식량")] SpecialFood = 201,
        [InspectorName("물")] Water = 202,
        [InspectorName("의료키트")] MedicKit = 203,
        [InspectorName("특별의료키트")] SpecialMedicKit = 204,
        [InspectorName("수리키트")] RepairKit = 205,
        [InspectorName("특별수리키트")] SpecialRepairKit = 206,
        [InspectorName("예비통신장비")] CommDevice = 207,
        [InspectorName("고급우주복")] NiceSpacesuit = 208,
        [InspectorName("총")] Gun = 209,
        [InspectorName("보드게임")] BoardGame = 210,
        [InspectorName("도끼")] Ax = 211,
        [InspectorName("쇠파이프")] Pipe = 212,
        [InspectorName("손전등")] Flashlight = 213,
        [InspectorName("행성탐사지도")] Map = 214,

        // 기타
        [InspectorName("아무것도 선택하지 않음")] Noting = 500,
        [InspectorName("누군가를 보냄")] Sendsomeone = 501,
        [InspectorName("맨손")] Hand = 502,

        None = 1000,
    }

    public enum EActionType
    {
        [InspectorName("스테이터스 악화")] WorsenStatus = 100,
        [InspectorName("스테이터스 치유")] RecoveryStatus = 101,
        [InspectorName("캐릭터 사망")] CharacterDaed = 102,

        [InspectorName("아이템 획득")] AddItem = 200,
        [InspectorName("아이템 삭제")] RemoveItem = 201,

        [InspectorName("엔딩 분기 진행")] AdvanceEndingProgress = 300,
        [InspectorName("엔딩 완료")] EndingComplete = 301,

    }
    #endregion

    [CreateAssetMenu(fileName = "MainEventData", menuName = "Scriptable Objects/MainEvent/MainEventData")]
    public class MainEventData : ScriptableObject
    {
        [SerializeField]
        private string _eventId;                                    // 고유 ID (ex. "yesorno_01")

        [SerializeField]
        private string _title;                                      // 엔딩 로그에 남길 타이틀

        [SerializeField]
        [TextArea(3, 10)]
        private List<string> _descriptions = new();                 // 랜덤 출력용 텍스트(이벤트 본문)

        [SerializeField]
        private List<Condition> _conditions = new();                // 이벤트 발생 조건

        [SerializeField]
        private EMainEventChoiceSchema _choiceSchema;               // 이벤트 선택지 구조 (YesOrNo, UseItem, ChooseSomeone 등)

        [SerializeField]
        private List<EventChoice> _choices;                         // 유저가 고를 수 있는 선택지


        public string EventId => _eventId;
        public string Title => _title;
        public IReadOnlyList<string> Descriptions => _descriptions;
        public IReadOnlyList<Condition> Conditions => _conditions;
        public EMainEventChoiceSchema ChoiceSchema => _choiceSchema;
        public List<EventChoice> Choices { get; set; }

        public string GetRandomDescription()
        {
            if (_descriptions == null || _descriptions.Count == 0)
                return string.Empty;

            int index = Random.Range(0, _descriptions.Count);
            return _descriptions[index];
        }

        public EventChoice GetEventChoice(EChoiceIcon choiceIcon) => _choices.FirstOrDefault(choice => choice.ChoiceIcon == choiceIcon);
    }

    [System.Serializable]
    public class Condition
    {
        [SerializeField]
        private EConditionType _conditionType;

        [SerializeField]
        private string _targetId;                   // 캐릭터 이름, 아이템 이름, 상태 이름 등

        [SerializeField]
        private string _parameter;                  // 비교할 속성 (ex: Status, State 등)

        [SerializeField]
        private string _value1;                     // 비교할 값 1

        [SerializeField]
        private string _value2;                     // 비교할 값 2

        public EConditionType ConditionType => _conditionType;
        public string TargetId => _targetId;
        public string Parameter => _parameter;
        public string Value1 => _value1;
        public string Value2 => _value2;

    }

    [System.Serializable]
    public class EventChoice
    {
        [SerializeField]
        private EChoiceIcon _choiceIcon;          // 선택 ID

        [SerializeField]
        private int _amount;                      // 필요 갯수

        [SerializeField]
        private List<EventResult> _results;       // 선택에 대한 결과 리스트

        public EChoiceIcon ChoiceIcon => _choiceIcon;
        public int Amount => _amount;
        public IReadOnlyList<EventResult> Results => _results;

        public EventChoice(EChoiceIcon choiceIcon, int amount = 1, List<EventResult> results = null)
        {
            _choiceIcon = choiceIcon;
            _amount = amount;
            _results = (results == null) ? new List<EventResult>() { new EventResult() } : results;
        }
    }

    [System.Serializable]
    public class EventResult
    {
        [SerializeField]
        private List<Condition> _conditions;      // 해당 결과가 발생할 조건

        [SerializeField]
        [TextArea(3, 10)]
        private string _resultText;               // 결과 텍스트

        [SerializeField]
        private List<EventAction> _actions;       // 결과 반영

        [SerializeField]
        [Range(0, 1)]
        private float _probability;               // 발생 확률 (총합 1.0 안 넘게)


        public IReadOnlyList<Condition> Conditions => _conditions;
        public string ResultText => _resultText;
        public IReadOnlyList<EventAction> Actions => _actions;
        public float Probability => _probability;

        public EventResult()
        {
            _conditions = new();
            _resultText = string.Empty;
            _actions = new();
            _probability = 1.0f;
        }
    }

    [System.Serializable]
    public class EventAction
    {
        [SerializeField]
        private EActionType _actionType;

        [SerializeField]
        private string _targetId;

        [SerializeField]
        private string _parameter;

        [SerializeField]
        private string _value;

        public EActionType ActionType => _actionType;
        public string TargetId => _targetId;
        public string Parameter => _parameter;
        public string Value => _value;
    }
}