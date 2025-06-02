using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.ItemEffect;

namespace WeWillSurvive
{
    [CreateAssetMenu(fileName = "ItemEffectData", menuName = "Scriptable Objects/ItemEffectData")]
    public class ItemEffectData : ScriptableObject
    {
        [field: SerializeField] public List<ScriptableItemEffect> ItemEffects { get; private set; }
    }
}
