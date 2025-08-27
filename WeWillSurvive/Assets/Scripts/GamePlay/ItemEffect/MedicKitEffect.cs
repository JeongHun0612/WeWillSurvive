using WeWillSurvive.Character;
using WeWillSurvive.Item;
using WeWillSurvive.ItemEffect;
using WeWillSurvive.Status;

namespace WeWillSurvive
{
    public class MedicKitEffect : IItemEffect
    {
        public EItem ItemType => EItem.MedicKit;

        public void Apply(CharacterBase target)
        {
            if (target == null)
                return;

            var status = target.Status.GetStatus<InjuryStatus>(EStatusType.Injury);
            status?.RecoverFully();
        }
    }
}