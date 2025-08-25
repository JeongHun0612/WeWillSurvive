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
                // ��� �����, �񸶸� ���� ȸ��
                var hungerStatus = character.Status.GetStatus<HungerStatus>(EStatusType.Hunger);
                hungerStatus?.RecoverFully();

                var thirstStatus = character.Status.GetStatus<ThirstStatus>(EStatusType.Thirst);
                thirstStatus?.RecoverFully();


                // N�ϵ��� �����, �񸶸� ��ȭ ���� ���� �߻�
                BuffManager.Instance.AddBuff(EBuffEffect.BlockHungerWorsen, _statusWorsenBlockDays);
                BuffManager.Instance.AddBuff(EBuffEffect.BlockThirstWorsen, _statusWorsenBlockDays);
            }
        }
    }
}
