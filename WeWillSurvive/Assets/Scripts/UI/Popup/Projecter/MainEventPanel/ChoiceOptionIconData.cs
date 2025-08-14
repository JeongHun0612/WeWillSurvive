using UnityEngine;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive
{
    [CreateAssetMenu(fileName = "ChoiceOptionIconData", menuName = "Scriptable Objects/ChoiceOptionIconData")]
    public class ChoiceOptionIconData : ScriptableObject
    {
        [field: SerializeField] public EChoiceIcon ChoiceType;
        [field: SerializeField] public Sprite NormalSprite;
        [field: SerializeField] public Sprite DisabledSprite;
    }
}