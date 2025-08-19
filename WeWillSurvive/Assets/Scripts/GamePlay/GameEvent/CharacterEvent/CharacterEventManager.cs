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

        [SerializeField] private List<CharacterEventPool> _characterEventPools;

        
        private int _globalDayCounter;                      // 이벤트 쿨타임 카운터
        private int _totalCharacterEventCount;              // 캐릭터 이벤트 총 발생 횟수

        private bool _isCharacterEventReady;                // 캐릭터 이벤트 준비 여부

        private Dictionary<ECharacter, CharacterEventProgress> _characterEventProgresses = new();


        //protected override void Awake()
        //{
        //    base.Awake();

        //    SetupCharacterEventProgress();
        //    ResetState();
        //}

        //public void ResetState()
        //{
        //    _globalDayCounter = 0;
        //    _totalCharacterEventCount = 0;

        //    _isCharacterEventReady = false;

        //    foreach (var characterEventProgress in _characterEventProgresses.Values)
        //    {
        //        characterEventProgress.ResetState();
        //    }
        //}

        //public void OnNewDay()
        //{
        //    if (_isCharacterEventReady)
        //        return;

        //    _globalDayCounter--;

        //    if (_globalDayCounter <= 0)
        //    {
        //        _globalDayCounter = 0;
        //        _isCharacterEventReady = true;
        //    }

        //    foreach (var characterEventProgress in _characterEventProgresses.Values)
        //    {
        //        characterEventProgress.OnNewDay();
        //    }
        //}
    }
}