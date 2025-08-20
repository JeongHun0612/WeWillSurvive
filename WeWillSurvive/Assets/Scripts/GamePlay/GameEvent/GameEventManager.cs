using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.CharacterEvent;
using WeWillSurvive.Core;
using WeWillSurvive.Ending;
using WeWillSurvive.Log;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive.GameEvent
{
    public enum EGameEventType
    {
        MainEvent,
        CharacterEvent,
        EndingEvent
    }

    public class GameEventManager : MonoSingleton<GameEventManager>
    {
        [Header("## EventPicker")]
        [SerializeField] private MainEventPicker _mainEventPicker;
        [SerializeField] private EndingEventPicker _endingEventPicker;
        [SerializeField] private CharacterEventPicker _characterEventPicker;

        private List<EGameEventType> _primaryPriority = new() { EGameEventType.EndingEvent, EGameEventType.MainEvent };

        private Dictionary<EGameEventType, IGameEventPicker> _pickerMap;

        private DailyMainEvent _dailyMainEvent;
        private DailyCharacterEvent _dailyCharacterEvent;

        public DailyMainEvent DailyMainEvent => _dailyMainEvent;
        public DailyCharacterEvent DailyCharacterEvent => _dailyCharacterEvent;

        public EndingEventPicker EndingEventPicker => _endingEventPicker;

        public async UniTask InitializeAsync()
        {
            // GameEventUtil 초기화
            await GameEventUtil.InitializeAsyn();

            // EventPicker 초기화
            _pickerMap = new()
            {
                { EGameEventType.MainEvent, _mainEventPicker },
                { EGameEventType.EndingEvent, _endingEventPicker },
                { EGameEventType.CharacterEvent, _characterEventPicker },
            };

            foreach (var picker in _pickerMap.Values) picker.Initialize();
            ResetState();

            await UniTask.Yield();
        }

        public void ResetState()
        {
            _dailyMainEvent = null;
            _dailyCharacterEvent = null;

            foreach (var picker in _pickerMap.Values) picker.ResetState();
        }

        public void OnDayComplete()
        {
            // 오늘 발생한 이벤트에 대한 결과 처리
            _dailyMainEvent?.ApplyEventResult();
            _dailyCharacterEvent?.ApplyEventResult();
        }

        public void OnNewDay()
        {
            foreach (var picker in _pickerMap.Values) picker.OnNewDay();

            SetDailyEvents();
        }

        public void SetDailyEvents()
        {
            _dailyMainEvent = null;
            _dailyCharacterEvent = null;

            // 1) 주 이벤트(Ending/Main) 중 발생하는 이벤트 선택
            _dailyMainEvent = _endingEventPicker.GetDailyMainEvent();
            if (_dailyMainEvent == null)
            {
                _dailyMainEvent = _mainEventPicker.GetDailyMainEvent();
            }

            // 2) 캐릭터 이벤트 선택
            _dailyCharacterEvent = _characterEventPicker.GetDailyCharacterEvent();
        }

        public void SelectedMainEventChoice(EventChoice choice)
        {
            if (_dailyMainEvent == null)
                return;

            _dailyMainEvent.DailyEventChoice = choice;
        }

        public void SelectedCharacterEventChoice(EventChoice choice)
        {
            if (_dailyCharacterEvent == null)
                return;

            _dailyCharacterEvent.DailyEventChoice = choice;
        }
    }

    [System.Serializable]
    public class DailyCharacterEvent : DailyEvent
    {
        public ECharacter Character { get; set; }

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public DailyCharacterEvent(MainEventData mainEventData, ECharacter character)
        {
            DailyEventData = mainEventData;
            Character = character;
        }

        protected override EventResult GetRandomEventResult()
        {
            var owner = CharacterManager.GetCharacter(Character);
            if (owner == null)
            {
                Debug.LogError($"{Character}에 해당하는 캐릭터를 찾을 수 없습니다.");
                return null;
            }

            return GameEventUtil.GetValidRandomEventResult(DailyEventChoice, owner.EventSuccessRate);
        }

        protected override void LogResult(string message)
        {
            LogManager.AddCharacterEventResultLog(message);
        }
    }

    [System.Serializable]
    public class DailyMainEvent : DailyEvent
    {
        public DailyMainEvent(MainEventData mainEventData)
        {
            DailyEventData = mainEventData;
        }

        protected override void LogResult(string message)
        {
            LogManager.AddMainEventResultLog(message);
        }
    }

    [System.Serializable]
    public abstract class DailyEvent
    {
        public MainEventData DailyEventData { get; protected set; }
        public EventChoice DailyEventChoice { get; set; }

        protected LogManager LogManager => ServiceLocator.Get<LogManager>();

        protected abstract void LogResult(string message);

        public virtual void ResetState()
        {
            DailyEventData = null;
            DailyEventChoice = null;
        }

        public virtual void ApplyEventResult()
        {
            if (DailyEventChoice == null)
            {
                Debug.Log("처리할 이벤트 선택지가 없습니다.");
                return;
            }

            Debug.Log($"[{DailyEventData.EventId}] 결과 추출");
            EventResult result = GetRandomEventResult();

            if (result == null)
            {
                Debug.LogWarning("선택한 Choice에 대해 유효한 Result를 찾지 못했습니다. (조건불충족 또는 확률 문제)");
                return;
            }

            // 이벤트 결과 적용
            foreach (var action in result.Actions)
            {
                GameEventUtil.ApplyResultAction(action);
            }

            // 로그 기록
            LogResult(result.ResultText);
        }

        protected virtual EventResult GetRandomEventResult()
        {
            return GameEventUtil.GetValidRandomEventResult(DailyEventChoice);
        }
    }
}
