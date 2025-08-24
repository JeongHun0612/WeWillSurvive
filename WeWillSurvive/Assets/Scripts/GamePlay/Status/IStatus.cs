
using UnityEngine;

namespace WeWillSurvive.Status
{
    public enum EStatusType
    {
        [InspectorName("배고픔")]
        Hunger,
        [InspectorName("목마름")]
        Thirst,
        [InspectorName("부상")]
        Injury,
        [InspectorName("불안함")]
        Anxious
    }

    public interface IStatus
    {
        public EStatusType StatusType { get; }

        public void OnNewDay();
        public void OnExpeditionResult();
        public void ResetStatus();
        public void WorsenStatus(int step = 1);
        public void RecoveryStatus(int step = 1);
        public void UpdateWorsenBlockDayCounter(int dayCounter);
        public bool HasWorsenBlock();
    }
}
