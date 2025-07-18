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

        public void OnNewDay(CharacterBase owner);
        public void ApplyRecovery();
    }
}
