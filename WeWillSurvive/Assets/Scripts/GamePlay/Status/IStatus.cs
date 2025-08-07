
using System.ComponentModel;

namespace WeWillSurvive.Status
{
    public enum EStatusType
    {
        [Description("�����")]
        Hunger,
        [Description("�񸶸�")]
        Thirst,
        [Description("�λ�")]
        Injury,
        [Description("�Ҿ���")]
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
