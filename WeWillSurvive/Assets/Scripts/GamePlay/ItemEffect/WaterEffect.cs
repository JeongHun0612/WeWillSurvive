using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Item;
using WeWillSurvive.Status;

namespace WeWillSurvive.ItemEffect
{
    public class WaterEffect : IItemEffect
    {
        public EItem ItemType => EItem.Water;

        public void Apply(CharacterBase target)
        {
            if (target == null)
                return;

            var status = target.Status.GetStatus<ThirstStatus>(EStatusType.Thirst);
            status?.RecoverFully();
        }
    }
}
