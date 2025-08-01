
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

        public void OnNewDay();
        public void OnExpeditionResult();
        public void ApplyRecovery();
    }
}
