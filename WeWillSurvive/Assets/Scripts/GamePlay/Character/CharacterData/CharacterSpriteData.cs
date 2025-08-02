using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive
{
    [CreateAssetMenu(fileName = "CharacterSpriteData", menuName = "Scriptable Objects/CharacterData/CharacterSpriteData")]
    public class CharacterSpriteData : ScriptableObject
    {
        [Header("## Morale Sprites (Seat)")]
        [SerializeField] private Sprite _normalSeated;
        [SerializeField] private Sprite _highSeated;
        [SerializeField] private Sprite _lowSeated;

        [Header("## Status Sprites (Seat)")]
        [Tooltip("다친 상태")]
        [SerializeField] private Sprite _injuredSeated;
        [Tooltip("공포 상태")]
        [SerializeField] private Sprite _scaredSeated;
        [Tooltip("미친 상태")]
        [SerializeField] private Sprite _madSeated;
        [Tooltip("다침 + 공포 상태")]
        [SerializeField] private Sprite _injuredScaredSeated;
        [Tooltip("다침 + 미침 상태")]
        [SerializeField] private Sprite _injuredMadSeated;
        [Tooltip("공포 + 미침 상태")]
        [SerializeField] private Sprite _scaredMadSeated;
        [Tooltip("다침 + 공포 + 미침 상태")]
        [SerializeField] private Sprite _injuredScaredMadSeated;

        [Header("## Morale Sprites (Stand)")]
        [SerializeField] private Sprite _normalStanding;
        [SerializeField] private Sprite _highStanding;
        [SerializeField] private Sprite _lowStanding;

        [Header("## Status Sprites (Stand)")]
        [Tooltip("다친 상태")]
        [SerializeField] private Sprite _injuredStanding;
        [Tooltip("공포 상태")]
        [SerializeField] private Sprite _scaredStanding;
        [Tooltip("미친 상태")]
        [SerializeField] private Sprite _madStanding;
        [Tooltip("다침 + 공포 상태")]
        [SerializeField] private Sprite _injuredScaredStanding;
        [Tooltip("다침 + 미침 상태")]
        [SerializeField] private Sprite _injuredMadStanding;
        [Tooltip("공포 + 미침 상태")]
        [SerializeField] private Sprite _scaredMadStanding;
        [Tooltip("다침 + 공포 + 미침 상태")]
        [SerializeField] private Sprite _injuredScaredMadStanding;

        public Sprite GetSeatedSprite(CharacterState state, EMorale morale)
        {
            bool isHurt = state.IsHurt;
            bool isAnxiety = state.IsAnxious;
            bool isMad = state.IsMad;

            // 상태 조합 우선순위
            if (isHurt && isAnxiety && isMad)
                return _injuredScaredMadSeated;

            if (isHurt && isMad)
                return _injuredMadSeated;

            if (isHurt && isAnxiety)
                return _injuredScaredSeated;

            if (isMad)
                return _madSeated;

            if (isAnxiety)
                return _scaredSeated;

            if (isHurt)
                return _injuredSeated;


            // 사기 기반 표정
            return morale switch
            {
                EMorale.VeryLow => _lowSeated,
                EMorale.Low => _lowSeated,
                EMorale.Normal => _normalSeated,
                EMorale.High => _highSeated,
                EMorale.VeryHigh => _highSeated,
                _ => null
            };
        }

        public Sprite GetStandingSprite(CharacterState state, EMorale morale)
        {
            bool isHurt = state.IsHurt;
            bool isAnxiety = state.IsAnxious;
            bool isMad = state.IsMad;

            // 상태 조합 우선순위
            if (isHurt && isAnxiety && isMad)
                return _injuredScaredMadStanding;

            if (isHurt && isMad)
                return _injuredMadStanding;

            if (isHurt && isAnxiety)
                return _injuredScaredStanding;

            if (isMad)
                return _madStanding;

            if (isAnxiety)
                return _scaredStanding;

            if (isHurt)
                return _injuredStanding;


            // 사기 기반 표정
            return morale switch
            {
                EMorale.VeryLow => _lowStanding,
                EMorale.Low => _lowStanding,
                EMorale.Normal => _normalStanding,
                EMorale.High => _highStanding,
                EMorale.VeryHigh => _highStanding,
                _ => null
            };
        }
    }
}
