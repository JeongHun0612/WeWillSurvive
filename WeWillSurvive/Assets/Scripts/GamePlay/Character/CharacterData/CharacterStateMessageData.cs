using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive
{
    [CreateAssetMenu(fileName = "CharacterStateMessageData", menuName = "Scriptable Objects/CharacterData/CharacterStateMessageData")]
    public class CharacterStateMessageData : ScriptableObject
    {
        [Header("## 상태 활성화 시 메시지")]
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


        [Header("## 상태 회복 시 메시지")]
        [SerializeField] private string _hungerResolvedMessage;
        [SerializeField] private string _starveResolvedMessage;
        [SerializeField] private string _thirstyResolvedMessage;
        [SerializeField] private string _dehydrateResolvedMessage;
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
                [EState.Starve] = _starveResolvedMessage,
                [EState.Thirsty] = _thirstyResolvedMessage,
                [EState.Dehydrate] = _dehydrateResolvedMessage,
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
    }
}
