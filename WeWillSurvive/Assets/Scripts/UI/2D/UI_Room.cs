using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_Room : UI_Popup
    {
        [SerializeField] private UI_Character[] _characterUIs;

        private Dictionary<ECharacter, UI_Character> _characterDicts = new();
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public async override UniTask InitializeAsync()
        {
            foreach (var characterUI in _characterUIs)
            {
                if (!_characterDicts.ContainsKey(characterUI.CharacterType))
                {
                    characterUI.Initialize();
                    _characterDicts.Add(characterUI.CharacterType, characterUI);
                }
            }

            await UniTask.Yield();
        }

        // 방 세팅
        public void SetupRoomUI(ECharacter owner)
        {
            foreach (var characterUI in _characterUIs)
            {
                characterUI.gameObject.SetActive(false);
            }

            if (_characterDicts.TryGetValue(owner, out var targetCharacterUI))
            {
                targetCharacterUI.UpdateCharacterImage();
            }
        }
    }
}