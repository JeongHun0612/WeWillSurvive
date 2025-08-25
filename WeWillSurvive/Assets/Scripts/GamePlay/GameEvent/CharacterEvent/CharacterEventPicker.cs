using System.Linq;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.GameEvent;

namespace WeWillSurvive.CharacterEvent
{
    public class CharacterEventPicker : GameEventPickerBase<ECharacter, CharacterEventProgress>
    {
        private int _totalCharacterEventCount;              // 캐릭터 이벤트 총 발생 횟수

        public override void ResetState()
        {
            base.ResetState();

            _totalCharacterEventCount = 0;
        }

        public DailyCharacterEvent GetDailyCharacterEvent()
        {
            // 쿨타임이 아직 안됐으면 이벤트 발생 X
            if (!_isEventReady || GameManager.Instance.Day < _availableDay)
                return null;

            if (_eventProgresses == null || _eventProgresses.Count <= 0)
                return null;

            CharacterEventProgress targetProgress;
            MainEventData selectedEvent;

            // 가장 처음은 Lead의 캐릭터 이벤트 발생
            if (_totalCharacterEventCount == 0)
            {
                targetProgress = GetCharacterEventProgress(ECharacter.Lead);
                selectedEvent = targetProgress.GetEventDataById("lead_01");
            }
            else
            {
                // 쿨타임이 다 됐고 캐릭터가 쉘터 안에 존재하는 프로그래스만 추출
                var readyProgresses = _eventProgresses.Values
                    .Where(progress => progress.IsReady && progress.Owner.IsInShelter)
                    .ToList();

                if (readyProgresses.Count == 0)
                {
                    Debug.LogWarning("준비 된 캐릭터 이벤트 프로그래스가 존재하지 않습니다.");
                    return null;
                }

                // 이벤트 발생 횟수가 가장 적은 프로그래스들 추출
                int minCount = readyProgresses.Min(progress => progress.EventTriggerCount);
                var minProgresses = readyProgresses
                    .Where(progress => progress.EventTriggerCount == minCount)
                    .ToList();

                // 랜덤한 프로그래스
                targetProgress = minProgresses[Random.Range(0, minProgresses.Count)];
                selectedEvent = targetProgress.GetValidRandomEvent();
            }

            _lastSelectedEvent = selectedEvent;

            // 상태 업데이트
            ResetEventCooldown();
            _totalCharacterEventCount++;

            Debug.Log($"[{targetProgress.Category} - {selectedEvent.EventId}] 캐릭터 이벤트 발생");

            return new DailyCharacterEvent(selectedEvent, targetProgress.Category);
        }

        private CharacterEventProgress GetCharacterEventProgress(ECharacter characterType)
        {
            return _eventProgresses.Values.FirstOrDefault(progress => progress.Category == characterType && progress.Owner.IsInShelter);
        }
    }

    [System.Serializable]
    public class CharacterEventProgress : EventProgress<ECharacter>
    {
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();
        public CharacterBase Owner { get; private set; }

        public override void ResetState()
        {
            base.ResetState();
            Owner = CharacterManager.GetCharacter(Category);
        }
    }
}