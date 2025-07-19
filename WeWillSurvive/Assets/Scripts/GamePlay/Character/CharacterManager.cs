using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Item;

namespace WeWillSurvive.Character
{
    public enum ECharacter
    {
        Lead,
        Cook,
        DrK,
        Bell
    }

    public class CharacterManager : IService
    {
        private readonly List<CharacterData> _characterDatas = new();
        private readonly Dictionary<int, EMorale> _moraleTable = new()
        {
            { 1, EMorale.VeryLow },
            { 2, EMorale.Low },
            { 3, EMorale.Low },
            { 4, EMorale.Normal },
        };

        public Dictionary<ECharacter, CharacterBase> Characters { get; private set; } = new();

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public async UniTask InitializeAsync()
        {
            // 캐릭터 데이터 오브젝트 로드
            foreach (ECharacter type in Enum.GetValues(typeof(ECharacter)))
            {
                var data = await ResourceManager.LoadAssetAsync<CharacterData>(type.ToString());

                if (data != null)
                {
                    data.Initialize();
                    _characterDatas.Add(data);
                }
            }

            // 캐릭터 초기화
            foreach (var characterData in _characterDatas)
            {
                if (!Characters.ContainsKey(characterData.Type))
                {
                    CharacterBase character = new();
                    character.Initialize(characterData);
                    Characters.Add(characterData.Type, character);
                }
            }
        }

        public void SettingCharacter()
        {
            // 캐릭터 데이터 초기화
            foreach (var character in Characters.Values)
            {
                character.ResetData();

                if (!ItemManager.HasItem(character.Data.ItemType))
                {
                    character.OnDead();
                }
                else
                {
                    ItemManager.RemoveItem(character.Data.ItemType);
                }
            }

            // Character 사기 설정 (남은 인원수에 따라 사기 결정)
            int aliveCount = AliveCharacterCount();

            if (_moraleTable.TryGetValue(aliveCount, out EMorale morale))
            {
                foreach (var character in Characters.Values)
                {
                    if (character.IsDead) continue;

                    character.SetMorale(morale);
                }
            }
            else
            {
                Debug.LogWarning("남은 인원수에 따른 사기(Morale) 값이 존재하지 않습니다.");
            }
        }

        public void UpdateCharacterStatus()
        {
            foreach (var character in Characters.Values)
            {
                if (character.IsDead) continue;

                character.OnNewDay();
            }

            // Test
            //GetCharacter(ECharacter.Bell).State.SetState(EState.Exploring);
        }

        public int AliveCharacterCount()
        {
            return Characters.Values.Count(character => character != null && !character.IsDead);
        }

        public List<CharacterBase> GetAllCharacters()
        {
            return Characters.Values.ToList();
        }

        public CharacterBase GetCharacter(ECharacter characterType)
        {
            if (Characters.TryGetValue(characterType, out var character))
            {
                return character;
            }

            return null;
        }
    }
}
