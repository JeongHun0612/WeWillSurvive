using WeWillSurvive.Character;
using WeWillSurvive.Item;
using WeWillSurvive.Status;

namespace WeWillSurvive.ItemEffect
{
    public class SpaceFoodEffect : IItemEffect
    {
        public EItem ItemType => EItem.Food;

        public void Apply(CharacterBase target)
        {
            if (target == null)
                return;

            var status = target.Status.GetStatus<HungerStatus>(EStatusType.Hunger);
            status?.RecoverFully();
        }
    }
}
