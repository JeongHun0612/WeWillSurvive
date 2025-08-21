using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Expedition;
using WeWillSurvive.GameEvent;

namespace WeWillSurvive.MainEvent
{
    public enum EMainEventCategory
    {
        [InspectorName("침입 이벤트")]
        Invasion = 0,

        [InspectorName("조사 이벤트")]
        Exploration = 1,

        [InspectorName("교환 이벤트")]
        Trade = 2,

        [InspectorName("시설 이벤트")]
        Facility = 3,

        [InspectorName("첫날 이벤트")]
        FirstDay = 100,

        [InspectorName("Noting 이벤트")]
        Noting = 101,
    }

    public class MainEventPicker : GameEventPickerBase<EMainEventCategory, MainEventProgress>
    {
        [SerializeField] private MainEventPool _notingEventPool;
        [SerializeField] private MainEventPool _firstDayEventPool;

        private MainEventProgress _notingEventProgress = new();
        private MainEventProgress _firstDayEventProgress = new();

        public override void Initialize()
        {
            base.Initialize();

            _notingEventProgress.Initialize(_notingEventPool);
            _firstDayEventProgress.Initialize(_firstDayEventPool);
        }

        public override void OnNewDay()
        {
            base.OnNewDay();

            _notingEventProgress.OnNewDay();
            _firstDayEventProgress.OnNewDay();
        }

        public DailyMainEvent GetDailyMainEvent()
        {
            MainEventProgress progress;

            bool isExpeditionReady = ExpeditionManager.Instance.CurrentState == EExpeditionState.Ready;
            if (isExpeditionReady)  // 탐사 준비 단계면 Noting 이벤트 발생
            {
                progress = _notingEventProgress;
            }
            else if (GameManager.Instance.Day == 1)     // 첫날 전용 이벤트 발생
            {
                progress = _firstDayEventProgress;
            }
            else
            {
                progress = GetCompletedRandomEventProgress();

                // progress가 null이거나 progress가 적절한 MainEventData를 반환하지 못하면 NotingEventProgress 할당
                var mainEvent = progress?.GetValidRandomEvent();
                if (mainEvent != null)
                {
                    Debug.Log($"[{name}] [{progress.Category}] 이벤트 발생");
                    return new DailyMainEvent(mainEvent);
                }

                progress = _notingEventProgress;
            }

            // 최종적으로 선택된 이벤트를 반환
            var selectedEvent = progress.GetValidRandomEvent();
            _lastSelectedEvent = selectedEvent;

            // 상태 업데이트
            ResetEventCooldown();

            Debug.Log($"[{name}] [{progress.Category}] 이벤트 발생");

            return new DailyMainEvent(selectedEvent);
        }
    }

    [System.Serializable]
    public class MainEventProgress : EventProgress<EMainEventCategory>
    {
    }
}