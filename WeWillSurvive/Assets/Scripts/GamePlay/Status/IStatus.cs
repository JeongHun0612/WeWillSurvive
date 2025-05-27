using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EStatusType
    {
        Hunger,
        Thirst,
        Hurt,
        Anxious
    }

    public interface IStatus
    {
        public EStatusType StatusType { get; }
        public int DaysInState { get; }

        public void OnNewDay(CharacterBase owner);
        public void ApplyRecovery();
        public string GetStatusDescription();
    }
}
