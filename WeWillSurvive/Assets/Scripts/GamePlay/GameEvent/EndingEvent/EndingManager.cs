using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive.Ending
{
    public enum EEndingType
    {
        // 구조 엔딩 (Rescue Endings)
        [InspectorName("다른 탐사대 구조")]
        RescueByOtherSurvivors,

        [InspectorName("착한 외계인 구조")]
        RescueByAliens,

        [InspectorName("로봇 구조")]
        RescueByRobots,

        // 사망 엔딩 (Death Endings)
        [InspectorName("모든 캐릭터 사망")]
        DeathByStarvation,
    }


    public class EndingManager : MonoSingleton<EndingManager>
    {
        public bool IsEnding { get; private set; }

        public void Ending(EEndingType endingType)
        {
            IsEnding = true;
            // TODO 게임 엔딩
            Debug.Log($"Game Ending!! [{endingType}]");
        }
    }
}
