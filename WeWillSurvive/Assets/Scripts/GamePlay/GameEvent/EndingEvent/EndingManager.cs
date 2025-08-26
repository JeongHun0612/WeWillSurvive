using UnityEngine;
using WeWillSurvive.Core;

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



        None = 100,
    }

    public class EndingManager : MonoSingleton<EndingManager>
    {
        public bool IsEnding { get; private set; }
        public EEndingType EndingType { get; private set; }

        public void ResetState()
        {
            IsEnding = false;
            EndingType = EEndingType.None;
        }

        public void Ending(EEndingType endingType)
        {
            IsEnding = true;
            EndingType = endingType;
        }
    }
}
