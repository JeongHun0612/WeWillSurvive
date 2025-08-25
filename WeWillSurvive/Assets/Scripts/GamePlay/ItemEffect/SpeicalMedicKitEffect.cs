using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.Status;

namespace WeWillSurvive.ItemEffect
{
    public class SpeicalMedicKitEffect : IItemEffect
    {
        public EItem ItemType => EItem.SpecialMedicKit;

        private readonly int _statusWorsenBlockDays = 3;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(CharacterBase target)
        {
            var characters = CharacterManager.GetCharactersInShelter();
            foreach (var character in characters)
            {
                var injuryStatus = character.Status.GetStatus<InjuryStatus>(EStatusType.Injury);
                injuryStatus?.RecoverFully();

                BuffManager.Instance.AddBuff(EBuffEffect.BlockInjuryWorsen, _statusWorsenBlockDays);
            }
        }
    }
}
