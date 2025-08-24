using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Item;

namespace WeWillSurvive.ItemEffect
{
    public interface IItemEffect
    {
        public EItem ItemType { get; }
        public void Apply(CharacterBase target);
    }
}
