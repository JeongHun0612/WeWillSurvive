using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Item;

namespace WeWillSurvive.ItemEffect
{
    public abstract class ScriptableItemEffect : ScriptableObject
    {
        [field:SerializeField] public EItem ItemType { get; private set; }
        public abstract void Apply(CharacterBase character);
    }
}
