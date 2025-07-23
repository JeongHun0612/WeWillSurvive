using UnityEngine;
using WeWillSurvive.Item;
using WeWillSurvive.Character;
using System.Collections;
using System.Collections.Generic;

namespace WeWillSurvive.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [Header("## Character Type and Name")]
        [SerializeField] private ECharacter _type;
        [SerializeField] private EItem _itemType;
        [SerializeField] private string _name;
        public ECharacter Type => _type;
        public EItem ItemType => _itemType;
        public string Name => _name;

        [Header("## 캐릭터 스프라이트 데이터")]
        public CharacterSpriteData SpriteData;

        [Header("## 캐릭터 상태에 대한 메시지 데이터")]
        public CharacterStateMessageData StateMessageData;

        [Header("## 캐릭터 탐사에 대한 메시지 데이터")]
        public CharacterExpeditionMessageData ExpeditionMessageData;


        public void Initialize()
        {
            StateMessageData.Initialize();
        }
    }
}
