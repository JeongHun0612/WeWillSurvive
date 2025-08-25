using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;

namespace WeWillSurvive.GameEvent
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

        [InspectorName("캐릭터 이벤트")]
        CharacterEvent = 50,

        [InspectorName("선택지 없음")]
        Noting = 100,
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

    public enum EOutcomeType
    { 
        [InspectorName("Normal")] Normal = 0,
        [InspectorName("대성공")] CriticalSuccess = 1,
        [InspectorName("성공")] Success = 2,
        [InspectorName("실패")] Failure = 3,
        [InspectorName("대실패")] CriticalFailure = 4,
    }

    #endregion

    [CreateAssetMenu(fileName = "MainEventData", menuName = "Scriptable Objects/MainEventData")]
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
        public List<EventChoice> Choices { get => _choices; set => _choices = value; }

        public string GetRandomDescription()
        {
            if (_descriptions == null || _descriptions.Count == 0)
                return string.Empty;

            int index = UnityEngine.Random.Range(0, _descriptions.Count);
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
        private int _requiredAmount;              // 필요 갯수

        [SerializeField]
        private string _choiceText;               // 선택지 텍스트

        [SerializeField]
        private List<EventResult> _results;       // 선택에 대한 결과 리스트

        public EChoiceIcon ChoiceIcon => _choiceIcon;
        public int RequiredAmount => _requiredAmount;
        public string ChoiceText => _choiceText;
        public IReadOnlyList<EventResult> Results => _results;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public EventChoice(EChoiceIcon choiceIcon, int requiredAmount = 1, string choiceText = "", List<EventResult> results = null)
        {
            _choiceIcon = choiceIcon;
            _requiredAmount = requiredAmount;
            _choiceText = choiceText;
            _results = (results == null) ? new List<EventResult>() { new EventResult() } : results;
        }

        public bool IsAvailable()
        {
            if (Enum.TryParse($"{this.ChoiceIcon}", out ECharacter characterType))
            {
                var character = CharacterManager.GetCharacter(characterType);
                // 해당 캐릭터가 존재하고, 쉼터에 있는지 확인
                return (character != null && character.IsInShelter);
            }
            // 2. 아이콘이 '아이템' 타입인지 확인
            else if (Enum.TryParse($"{this.ChoiceIcon}", out EItem item))
            {
                // 요구량이 0이거나, 요구량만큼 아이템을 가지고 있는지 확인
                return this.RequiredAmount == 0 || ItemManager.HasItem(item, this.RequiredAmount);
            }
            // 3. 위 두 조건에 해당하지 않는 다른 모든 타입의 아이콘
            else
            {
                // 특별한 조건이 없으므로 항상 활성화
                return true;
            }
        }
    }

    [System.Serializable]
    public class EventResult
    {
        [SerializeField]
        private EOutcomeType _outComeType;       // 결과 타입

        [SerializeField]
        private List<Condition> _conditions;      // 해당 결과가 발생할 조건

        [SerializeField]
        [TextArea(3, 10)]
        private List<string> _resultTemplates;    // 결과 텍스트 템플릿

        [SerializeField]
        [TextArea(3, 10)]
        private string _resultText;               // 결과 텍스트

        [SerializeField]
        private List<EventAction> _actions;       // 결과 반영

        [SerializeField]
        private bool _isAffectedByStats;          // 캐릭터 스텟에 영향을 받는가

        [SerializeField]
        [Range(0, 1)]
        private float _probability;               // 발생 확률 (합이 1.0을 넘으면 안됌)

        public EOutcomeType OutcomeType => _outComeType;
        public IReadOnlyList<Condition> Conditions => _conditions;
        public IReadOnlyList<string> ResultTemplates => _resultTemplates;
        public string ResultText => _resultText;
        public IReadOnlyList<EventAction> Actions => _actions;
        public bool IsAffectedByStats => _isAffectedByStats;
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