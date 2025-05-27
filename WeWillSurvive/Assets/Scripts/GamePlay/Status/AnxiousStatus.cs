using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public class AnxiousStatus : IStatus
    {
        public EStatusType StatusType => EStatusType.Anxious;

        public int DaysInState { get; private set; } = 0;

        public void OnNewDay(CharacterBase owner)
        {
        }

        public void ApplyRecovery()
        {
        }

        public string GetStatusDescription()
        {
            return string.Empty;
        }
    }
}
