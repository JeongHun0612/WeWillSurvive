using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive.Character
{
    public class CharacterManager : MonoSceneSingleton<CharacterManager>
    {
        private Dictionary<ECharacterType, CharacterBase> _characters = new();

        protected override void Awake()
        {
            base.Awake();

            AddCharacter(ECharacterType.Lead);
            AddCharacter(ECharacterType.Cook);
            AddCharacter(ECharacterType.DrK);
            AddCharacter(ECharacterType.Bell);
        }

        public void AddCharacter(ECharacterType type)
        {
            if (!_characters.ContainsKey(type))
            {
                _characters.Add(type, new CharacterBase(type));
            }
        }

        public void OnDayPassed()
        {
            foreach (var character in _characters.Values)
            {
                character.OnDayPassed();
            }
        }

        public CharacterBase GetCharacter(ECharacterType type)
        {
            if (_characters.TryGetValue(type, out var character))
            {
                return character;
            }

            return null;
        }
    }
}
