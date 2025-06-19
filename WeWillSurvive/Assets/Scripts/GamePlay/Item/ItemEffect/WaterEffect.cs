using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Status;

namespace WeWillSurvive.ItemEffect
{
    [CreateAssetMenu(fileName = "WaterEffect", menuName = "Scriptable Objects/ItemEffect/WaterEffect")]
    public class WaterEffect : ScriptableItemEffect
    {
        [field: SerializeField] public float Value { get; private set; }

        public override void Apply(CharacterBase character)
        {
            var status = character.Status.GetStatus<HungerStatus>(EStatusType.Hunger);
            status?.ApplyRecovery(Value);
        }
    }
}
