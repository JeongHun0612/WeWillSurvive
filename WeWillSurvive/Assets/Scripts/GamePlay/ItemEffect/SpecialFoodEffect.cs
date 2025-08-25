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

            foreach (var character in characters)
            {
                // 모든 배고픔, 목마름 상태 회복
                var hungerStatus = character.Status.GetStatus<HungerStatus>(EStatusType.Hunger);
                hungerStatus?.RecoverFully();

                var thirstStatus = character.Status.GetStatus<ThirstStatus>(EStatusType.Thirst);
                thirstStatus?.RecoverFully();


                // N일동안 배고픔, 목마름 악화 차단 버프 발생
                BuffManager.Instance.AddBuff(EBuffEffect.BlockHungerWorsen, _statusWorsenBlockDays);
                BuffManager.Instance.AddBuff(EBuffEffect.BlockThirstWorsen, _statusWorsenBlockDays);
            }
        }
    }
}
