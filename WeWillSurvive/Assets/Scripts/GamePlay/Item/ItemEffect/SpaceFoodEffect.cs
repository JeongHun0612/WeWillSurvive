using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Status;

namespace WeWillSurvive.ItemEffect
{
    [CreateAssetMenu(fileName = "SpaceFoodEffect", menuName = "Scriptable Objects/ItemEffect/SpaceFoodEffect")]
    public class SpaceFoodEffect : ScriptableItemEffect
    {
        [field: SerializeField] public float Value { get; private set; }

        public override void Apply(CharacterBase character)
        {
            var status = character.Status.GetStatus<HungerStatus>(EStatusType.Hunger);
            status?.ApplyRecovery(Value);
        }
    }
}
