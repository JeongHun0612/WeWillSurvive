using UnityEngine;
using WeWillSurvive.GameEvent;

namespace WeWillSurvive
{
    [CreateAssetMenu(fileName = "ChoiceIconData", menuName = "Scriptable Objects/ChoiceIconData")]
    public class ChoiceIconData : ScriptableObject
    {
        [field: SerializeField] public EChoiceIcon ChoiceIcon;
        [field: SerializeField] public Sprite NormalSprite;
        [field: SerializeField] public Sprite DisabledSprite;
    }
}