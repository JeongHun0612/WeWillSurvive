
using System.ComponentModel;

namespace WeWillSurvive.Status
{
    public enum EStatusType
    {
        [Description("배고픔")]
        Hunger,
        [Description("목마름")]
        Thirst,
        [Description("부상")]
        Injury,
        [Description("불안함")]
        Anxious
    }

    public interface IStatus
    {
        public EStatusType StatusType { get; }

        public void OnNewDay();
        public void OnExpeditionResult();
        public void WorsenStatus(int step = 1);
        public void RecoveryStatus(int step = 1);
    }
}
