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
            // GameEventUtil �ʱ�ȭ
            await GameEventUtil.InitializeAsyn();

            // EventPicker �ʱ�ȭ
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
            // ���� �߻��� �̺�Ʈ�� ���� ��� ó��
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

            // 1) �� �̺�Ʈ(Ending/Main) �� �߻��ϴ� �̺�Ʈ ����
            _dailyMainEvent = _endingEventPicker.GetDailyMainEvent();
            if (_dailyMainEvent == null)
            {
                _dailyMainEvent = _mainEventPicker.GetDailyMainEvent();
            }

            // 2) ĳ���� �̺�Ʈ ����
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
                Debug.LogError($"{Character}�� �ش��ϴ� ĳ���͸� ã�� �� �����ϴ�.");
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
                Debug.Log("ó���� �̺�Ʈ �������� �����ϴ�.");
                return;
            }

            Debug.Log($"[{DailyEventData.EventId}] ��� ����");
            EventResult result = GetRandomEventResult();

            if (result == null)
            {
                Debug.LogWarning("������ Choice�� ���� ��ȿ�� Result�� ã�� ���߽��ϴ�. (���Ǻ����� �Ǵ� Ȯ�� ����)");
                return;
            }

            // �̺�Ʈ ��� ����
            foreach (var action in result.Actions)
            {
                GameEventUtil.ApplyResultAction(action);
            }

            // �α� ���
            LogResult(result.ResultText);
        }

        protected virtual EventResult GetRandomEventResult()
        {
            return GameEventUtil.GetValidRandomEventResult(DailyEventChoice);
        }
    }
}
