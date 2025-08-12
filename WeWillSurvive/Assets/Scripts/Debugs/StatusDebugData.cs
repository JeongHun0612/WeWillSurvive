using UnityEngine;
using WeWillSurvive.Status;

namespace WeWillSurvive
{
    [CreateAssetMenu(fileName = "StatusDebugData", menuName = "Scriptable Objects/Debug/StatusDebugData")]
    public class StatusDebugData : ScriptableObject
    {
        [Header("허기")]
        public EHungerLevel Hunger;

        [Header("갈증")]
        public EThirstLevel Thirst;

        [Header("부상/질병")]
        public EInjuredLevel Injury;

        [Header("불안/공황")]
        public EAnxiousLevel Anxiety;
    }
}
