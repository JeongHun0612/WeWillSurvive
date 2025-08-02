using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.ItemEffect;
using WeWillSurvive.Status;

namespace WeWillSurvive
{
    [CreateAssetMenu(fileName = "MedicKitEffect", menuName = "Scriptable Objects/ItemEffect/MedicKitEffect")]
    public class MedicKitEffect : ScriptableItemEffect
    {
        public override void Apply(CharacterBase character)
        {
            if (character == null)
                return;

            var status = character.Status.GetStatus<InjuryStatus>(EStatusType.Injury);
            status?.ApplyRecovery();
        }
    }
}
