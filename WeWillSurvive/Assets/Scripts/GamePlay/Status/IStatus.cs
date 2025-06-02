using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public enum EStatusType
    {
        Hunger,
        Thirst,
        Injury,
        Anxious
    }

    public interface IStatus
    {
        public EStatusType StatusType { get; }
        public float MaxValue { get; }
        public float CurrentValue { get; }
        public float DecreasePerDay { get; }

        public void OnNewDay(CharacterBase owner);
        public void ApplyRecovery(float value);
    }
}
