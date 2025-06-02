using UnityEngine;
using WeWillSurvive.Item;
using static Define;

namespace WeWillSurvive.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [field: SerializeField] public ECharacter Type { get; private set; }
        [field: SerializeField] public Item.EItem ItemType { get; private set; }
        [field: SerializeField] public string Name { get; private set; }

        [Header("## Character Status")]
        [field: SerializeField] public float MaxHunger { get; private set; }
        [field: SerializeField] public float MaxThirst { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }

        [Header("## Morale Sprites")]
        [SerializeField] private Sprite _normal;
        [SerializeField] private Sprite _high;
        [SerializeField] private Sprite _low;

        [Header("## Status Sprites")]
        [Tooltip("다친 상태")]
        [SerializeField] private Sprite _injured;
        [Tooltip("공포 상태")]
        [SerializeField] private Sprite _scared;
        [Tooltip("미친 상태")]
        [SerializeField] private Sprite _mad;
        [Tooltip("다침 + 미침 상태")]
        [SerializeField] private Sprite _injuredMad;
        [Tooltip("다침 + 공포 상태")]
        [SerializeField] private Sprite _injuredScared;
        [Tooltip("공포 + 미침 상태")]
        [SerializeField] private Sprite _scaredMad;
        [Tooltip("다침 + 공포 + 미침 상태")]
        [SerializeField] private Sprite _injuredScaredMad;

        public Sprite GetSprite(ECharacterState state, ECharacterMorale morale)
        {
            var isHurt = ECharacterState.Hurt | ECharacterState.Sick;
            var isAnxiety = ECharacterState.Anxiety | ECharacterState.Panic;
            var isMad = ECharacterState.Mad;

            // 다침 + 공포 + 미침
            if ((state & isHurt) != 0 &&
                (state & isAnxiety) != 0 &&
                (state & isMad) != 0)
                return _injuredScaredMad;

            // 다침 + 미침
            if ((state & isHurt) != 0 &&
                (state & isMad) != 0)
                return _injuredMad;

            // 다침 + 공포
            if ((state & isHurt) != 0 &&
                (state & isAnxiety) != 0)
                return _injuredScared;

            // 미침
            if ((state & isMad) != 0)
                return _mad;

            // 공포
            if ((state & isAnxiety) != 0)
                return _scared;

            // 다침
            if ((state & isHurt) != 0)
                return _injured;


            // 사기 기반 표정
            return morale switch
            {
                ECharacterMorale.VeryLow => _low,
                ECharacterMorale.Low => _low,
                ECharacterMorale.Normal => _normal,
                ECharacterMorale.High => _high,
                ECharacterMorale.VeryHigh => _high,
                _ => null
            };
        }
    }
}
