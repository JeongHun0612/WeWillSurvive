using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Item;

namespace WeWillSurvive.ItemEffect
{
    public abstract class ScriptableItemEffect : ScriptableObject, IItemEffect
    {
        [field: SerializeField] public EItem Item { get; private set; }
        public abstract void Apply(CharacterBase character);
    }
}
