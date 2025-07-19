using UnityEngine;
using WeWillSurvive.Item;
using WeWillSurvive.Character;
using System.Collections;
using System.Collections.Generic;

namespace WeWillSurvive.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [Header("## Character Type and Name")]
        [SerializeField] private ECharacter _type;
        [SerializeField] private EItem _itemType;
        [SerializeField] private string _name;
        public ECharacter Type => _type;
        public EItem ItemType => _itemType;
        public string Name => _name;

        [Header("## Character Status")]
        [SerializeField] private float _maxHunger;
        [SerializeField] private float _maxThirst;
        [SerializeField] private float _maxHealth;
        public float MaxHunger => _maxHunger;
        public float MaxThirst => _maxThirst;
        public float MaxHealth => _maxHealth;


        [Header("## Morale Sprites")]
        [SerializeField] private Sprite _normal;
        [SerializeField] private Sprite _high;
        [SerializeField] private Sprite _low;

        [Header("## Status Sprites (Main)")]
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

        [Header("## State Active Message")]
        [SerializeField] private string _hungerActiveMessage;
        [SerializeField] private string _starveActiveMessage;
        [SerializeField] private string _thirstyActiveMessage;
        [SerializeField] private string _dehydrateActiveMessage;
        [SerializeField] private string _injuredActiveMessage;
        [SerializeField] private string _sickActiveMessage;
        [SerializeField] private string _anxiousActiveMessage;
        [SerializeField] private string _panicActiveMessage;
        [SerializeField] private string _madActiveMessage;
        [SerializeField] private string _deadActiveMessage;


        [Header("## State Resolve Message")]
        [SerializeField] private string _hungerResolvedMessage;
        //[SerializeField] private string _starveResolvedMessage;
        [SerializeField] private string _thirstyResolvedMessage;
        //[SerializeField] private string _dehydrateResolvedMessage;
        [SerializeField] private string _injuredResolvedMessage;
        [SerializeField] private string _sickResolvedMessage;
        [SerializeField] private string _anxiousResolvedMessage;
        [SerializeField] private string _panicResolvedMessage;
        [SerializeField] private string _madResolvedMessage;

        private Dictionary<EState, string> _stateActiveMessageDict = new();
        private Dictionary<EState, string> _stateResolvedMessageDict = new();

        public void Initialize()
        {
            _stateActiveMessageDict = new()
            {
                [EState.Hungry] = _hungerActiveMessage,
                [EState.Starve] = _starveActiveMessage,
                [EState.Thirsty] = _thirstyActiveMessage,
                [EState.Dehydrate] = _dehydrateActiveMessage,
                [EState.Injured] = _injuredActiveMessage,
                [EState.Sick] = _sickActiveMessage,
                [EState.Anxious] = _anxiousActiveMessage,
                [EState.Panic] = _panicActiveMessage,
                [EState.Mad] = _madActiveMessage,
                [EState.Dead] = _deadActiveMessage,
            };

            _stateResolvedMessageDict = new()
            {
                [EState.Hungry] = _hungerResolvedMessage,
                [EState.Starve] = _hungerResolvedMessage,
                [EState.Thirsty] = _thirstyResolvedMessage,
                [EState.Dehydrate] = _thirstyResolvedMessage,
                [EState.Injured] = _injuredResolvedMessage,
                [EState.Sick] = _sickResolvedMessage,
                [EState.Anxious] = _anxiousResolvedMessage,
                [EState.Panic] = _panicResolvedMessage,
                [EState.Mad] = _madResolvedMessage,
            };
        }

        public string GetStateActiveMessage(EState state)
        {
            if (!_stateActiveMessageDict.TryGetValue(state, out string message))
            {
                Debug.LogError($"[{state}] ActiveMessage not found.");
                return string.Empty;
            }

            return message;
        }

        public string GetStateResolvedMessage(EState state)
        {
            if (!_stateResolvedMessageDict.TryGetValue(state, out string message))
            {
                Debug.LogError($"[{state}] ResolvedMessage not found.");
                return string.Empty;
            }

            return message;
        }

        public Sprite GetMainSprite(EState state, EMorale morale)
        {
            bool isHurt = (state & (EState.Injured | EState.Sick)) != 0;
            bool isAnxiety = (state & (EState.Anxious | EState.Panic)) != 0;
            bool isMad = (state & EState.Mad) != 0;

            // 상태 조합 우선순위
            if (isHurt && isAnxiety && isMad)
                return _injuredScaredMad;

            if (isHurt && isMad)
                return _injuredMad;

            if (isHurt && isAnxiety)
                return _injuredScared;

            if (isMad)
                return _mad;

            if (isAnxiety)
                return _scared;

            if (isHurt)
                return _injured;


            // 사기 기반 표정
            return morale switch
            {
                EMorale.VeryLow => _low,
                EMorale.Low => _low,
                EMorale.Normal => _normal,
                EMorale.High => _high,
                EMorale.VeryHigh => _high,
                _ => null
            };
        }
    }
}
