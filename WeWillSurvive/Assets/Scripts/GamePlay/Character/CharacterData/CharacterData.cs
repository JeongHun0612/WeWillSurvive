using UnityEngine;
using WeWillSurvive.Item;

namespace WeWillSurvive.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [Header("## Character Type and Name")]

        [SerializeField] private ECharacter _type;              // 캐릭터 타입
        [SerializeField] private EItem _itemType;               // 캐릭터(아이템) 타입
        [SerializeField] private string _name;                  // 캐릭터 이름
        [SerializeField] private float _baseEventRate;          // 캐릭터 이벤트 기본 성공 확률
        [SerializeField] private float _normalStateModifier;    // 캐릭터 정상 상태 시 성공 확률

        public ECharacter Type => _type;
        public EItem ItemType => _itemType;
        public string Name => _name;
        public float BaseEventRate => _baseEventRate;
        public float NormalStateModifier => _normalStateModifier;


        [Header("## 캐릭터 스프라이트 데이터")]
        public CharacterSpriteData SpriteData;

        [Header("## 캐릭터 상태에 대한 메시지 데이터")]
        public CharacterStateMessageData StateMessageData;

        [Header("## 캐릭터 탐사에 대한 메시지 데이터")]
        public CharacterExpeditionMessageData ExpeditionMessageData;

        [Header("## 캐릭터 스테이터스 Debug 데이터")]
        public StatusDebugData StatusDebugData;

        public void Initialize()
        {
            StateMessageData.Initialize();
        }
    }
}
