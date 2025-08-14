using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive.CharacterEvent
{
    public class CharacterEventManager : MonoSingleton<CharacterEventManager>
    {
        // 이벤트에 대한 전역 발생 주기
        private const int MIN_GLOBAL_DAY_COUNT = 4;
        private const int MAX_GLOBAL_DAY_COUNT = 6;

        [SerializeField] private List<CharacterEventData> _characterEventDatas = new();

        private int _globalDayCounter; // 이벤트 쿨타임 카운터

        private bool _isCharacterEventReady;


        private Dictionary<ECharacter, CharacterEvent> _characterEvents = new();

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        protected override void Awake()
        {
            base.Awake();

            SetupCharacterEventDatas();
        }

        public void ResetState()
        {
            _globalDayCounter = 0;
            _isCharacterEventReady = false;
        }

        public void OnNewDay()
        {
            if (_isCharacterEventReady)
                return;

            _globalDayCounter--;

            if (_globalDayCounter <= 0)
            {
                _globalDayCounter = 0;
                _isCharacterEventReady = true;
            }
        }

        public MainEventData GetDailyCharacterEvent()
        {
            // 쿨타임이 아직 안됐으면 이벤트 발생 X
            if (!_isCharacterEventReady)
                return null;


            // 가장 처음은 Lead의 캐릭터 이벤트 발생
            if (GameManager.Instance.Day == 2)
            {
                var characterEvent = GetCharacterEvent(ECharacter.Lead);
                return characterEvent.GetRandomEvent();
            }

            // 우주선에 존재하고 캐릭터 이벤트가 한번도 발생하지 않은 캐릭터들 반환
            var readyCharacters = CharacterManager.GetAllCharacters()
                .Where(character => character != null && character.IsInShelter && character.TotalCharacterEventCount == 0)
                .ToList();

            if (readyCharacters.Count == 0)
            {
                // TODO 이전 캐릭터를 제외한 랜덤한 캐릭터 이벤트 발생
            }
            else
            {
                // TODO 반환된 캐릭터들 중 랜덤한 캐릭터 이벤트 발생
                int randomIndex = UnityEngine.Random.Range(0, readyCharacters.Count);
                var character = readyCharacters[randomIndex];

            }


            // 상태 업데이트
            _globalDayCounter = Random.Range(MIN_GLOBAL_DAY_COUNT, MAX_GLOBAL_DAY_COUNT + 1);
            _isCharacterEventReady = false;


            return null;
        }

        private MainEventData GetRandomEvent(IReadOnlyList<MainEventData> datas)
        {
            if (!_isCharacterEventReady || datas == null || datas.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, datas.Count);
            return datas[randomIndex];
        }

        private void UpdateCharacterEventCount(ECharacter characterType)
        {
            var character = CharacterManager.GetCharacter(characterType);
        }

        private CharacterEvent GetCharacterEvent(ECharacter characterType)
        {
            if (!_characterEvents.TryGetValue(characterType, out var characterEvent))
            {
                Debug.LogWarning($"[{characterType}] 타입의 CharacterEvent를 찾을 수 없습니다.");
                return null;
            }

            return characterEvent;
        }

        private void SetupCharacterEventDatas()
        {
            foreach (var characterEventData in _characterEventDatas)
            {
                if (!_characterEvents.ContainsKey(characterEventData.CharacterType))
                {
                    _characterEvents.Add(characterEventData.CharacterType, new CharacterEvent(characterEventData));
                }
            }
        }
    }

    [System.Serializable]
    public class CharacterEvent
    {
        private CharacterEventData _characterEventData;
        private int _totalEventCount = 0;

        public ECharacter CharacterType => _characterEventData.CharacterType;
        public IReadOnlyList<MainEventData> CharacterEventDatas => _characterEventData.CharacterEventDatas;
        public int TotalEventCount => _totalEventCount;

        public CharacterEvent(CharacterEventData data)
        {
            _characterEventData = data;
            ResetState();
        }

        public void ResetState()
        {
            _totalEventCount = 0;
        }

        public MainEventData GetRandomEvent()
        {
            if (CharacterEventDatas == null || CharacterEventDatas.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, CharacterEventDatas.Count);
            return CharacterEventDatas[randomIndex];
        }
    }
}