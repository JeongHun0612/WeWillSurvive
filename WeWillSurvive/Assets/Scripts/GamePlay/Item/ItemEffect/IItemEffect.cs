using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive.ItemEffect
{
    public class ItemEffectContext
    {
        public CharacterBase Target;

        private List<CharacterBase> _allCharacters;
        public List<CharacterBase> AllCharacters =>
            _allCharacters ??= CharacterManager.GetAllCharacters();

        private Character.CharacterManager CharacterManager => ServiceLocator.Get<Character.CharacterManager>();
    }

    public interface IItemEffect
    {
        public void Apply(CharacterBase character);
    }

}
