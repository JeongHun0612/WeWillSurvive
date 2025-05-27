using UnityEngine;

namespace WeWillSurvive.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        [field: SerializeField] public ECharacterType Type { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite[] MoraleSprites { get; private set; }
    }
}
