using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.Status;

namespace WeWillSurvive.ItemEffect
{
    public class SpecialFoodEffect : IItemEffect
    {
        public EItem ItemType => EItem.SpecialFood;

        private readonly int _statusWorsenBlockDays = 3;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Apply(CharacterBase target)
        {
            var characters = CharacterManager.GetCharactersInShelter();

            Debug.Log("SpeicalFood Apply : " + characters.Count);

            foreach (var character in characters)
            {
                var hungerStatus = character.Status.GetStatus<HungerStatus>(EStatusType.Hunger);
                hungerStatus?.RecoverFully();
                hungerStatus?.UpdateDayCounter(-_statusWorsenBlockDays);

                var thirstStatus = character.Status.GetStatus<ThirstStatus>(EStatusType.Thirst);
                thirstStatus?.RecoverFully();
                thirstStatus?.UpdateDayCounter(-_statusWorsenBlockDays);
            }
        }
    }
}
