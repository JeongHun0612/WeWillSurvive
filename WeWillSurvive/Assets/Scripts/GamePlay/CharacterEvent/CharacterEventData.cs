using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive.CharacterEvent
{
    [CreateAssetMenu(fileName = "CharacterEventData", menuName = "Scriptable Objects/CharacterEventData")]
    public class CharacterEventData : ScriptableObject
    {
        [SerializeField] private ECharacter _characterType;
        [SerializeField] private List<MainEventData> _characterEventDatas = new();

        public ECharacter CharacterType => _characterType;
        public IReadOnlyList<MainEventData> CharacterEventDatas => _characterEventDatas;
    }
}
