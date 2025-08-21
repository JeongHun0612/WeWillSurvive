using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.GameEvent;

namespace WeWillSurvive.Ending
{
    public class EndingEventPicker : GameEventPickerBase<EEndingType, EndingEventProgress>
    {
        public DailyMainEvent GetDailyMainEvent()
        {
            // 쿨타임이 아직 안됐으면 이벤트 발생 X
            if (!_isEventReady || GameManager.Instance.Day < _availableDay)
                return null;

            // 쿨타임이 다 된 엔딩프로그래스 중 하나 반환
            var endingProgress = GetCompletedRandomEventProgress();

            if (endingProgress == null)
            {
                Debug.LogWarning("준비된 엔딩 프로그래스가 존재하지 않습니다.");
                return null;
            }

            // 선택된 엔딩의 이벤트 데이터 가져오기
            var selectedEvent = endingProgress.GetDailyEvent();
            _lastSelectedEvent = selectedEvent;

            // 상태 업데이트
            ResetEventCooldown();

            Debug.Log($"[{endingProgress.Category} - {selectedEvent.EventId}] 엔딩 이벤트 발생");

            return new DailyMainEvent(selectedEvent);
        }

        public bool AdvanceEndingProgress(EEndingType endingType)
        {
            if (_eventProgresses.TryGetValue(endingType, out EndingEventProgress progress))
            {
                progress.CompleteEvent();
                return true;
            }
            else
            {
                Debug.LogError($"[{name}] [{endingType}] 타입의 엔딩이 등록되어 있지 않습니다!");
                return false;
            }
        }
    }


    [System.Serializable]
    public class EndingEventProgress : EventProgress<EEndingType>
    {
        public int CurrentEventIndex { get; private set; }

        public override void ResetState()
        {
            base.ResetState();

            CurrentEventIndex = 0;
        }

        public void CompleteEvent()
        {
            CurrentEventIndex = Mathf.Min(CurrentEventIndex + 1, Events.Count - 1);

            Debug.Log($"엔딩 [{Category}]의 진행도가 [{CurrentEventIndex + 1}/{Events.Count}]로 업데이트되었습니다.");

            if (CurrentEventIndex == Events.Count - 1)
            {
                // TODO Ending 완료
            }
        }

        public MainEventData GetDailyEvent()
        {
            EventTriggerCount++;
            ResetDayCounter();

            // 현재 엔딩 index에 맞는 이벤트를 반환
            return Events[CurrentEventIndex];
        }
    }
}
