using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Status;

namespace WeWillSurvive.ItemEffect
{
    [CreateAssetMenu(fileName = "WaterEffect", menuName = "Scriptable Objects/ItemEffect/WaterEffect")]
    public class WaterEffect : ScriptableItemEffect
    {
        public override void Apply(CharacterBase character)
        {
            if (character == null)
                return;

            var status = character.Status.GetStatus<ThirstStatus>(EStatusType.Thirst);
            status?.ApplyRecovery();
        }
    }
}
