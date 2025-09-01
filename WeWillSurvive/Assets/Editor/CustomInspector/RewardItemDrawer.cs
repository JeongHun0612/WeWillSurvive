using UnityEditor;
using UnityEngine;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    [CustomPropertyDrawer(typeof(ExpeditionRewardItem))]
    public class RewardItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            var itemProp = property.FindPropertyRelative("_item");
            var minAmountProp = property.FindPropertyRelative("_minAmount");
            var maxAmountProp = property.FindPropertyRelative("_maxAmount");

            EditorGUI.PropertyField(rect, itemProp, new GUIContent("보상 아이템"));
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            var itemType = (EItem)itemProp.intValue;
            if (itemType == EItem.Food || itemType == EItem.Water)
            {
                PropertyUtil.DrawIntField(ref rect, "최소값", minAmountProp);
                PropertyUtil.DrawIntField(ref rect, "최대값", maxAmountProp);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2) * 3;
        }
    }
}
