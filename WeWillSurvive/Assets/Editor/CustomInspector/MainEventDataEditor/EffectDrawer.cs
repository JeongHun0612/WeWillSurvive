using UnityEditor;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Item;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Status;
using WeWillSurvive.Util;

namespace WeWillSurvive
{
    [CustomPropertyDrawer(typeof(EventEffect))]
    public class EffectDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            var effectTypeTypeProp = property.FindPropertyRelative("effectType");
            var targetIdProp = property.FindPropertyRelative("targetId");
            var parameterProp = property.FindPropertyRelative("parameter");
            var valueProp = property.FindPropertyRelative("value");

            // EffectType
            var descriptions = EnumUtil.GetEnumDescriptions<EEffectType>();
            effectTypeTypeProp.enumValueIndex = EditorGUI.Popup(rect, "이벤트 결과 타입", effectTypeTypeProp.enumValueIndex, descriptions);
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            var conditionType = (EEffectType)effectTypeTypeProp.enumValueIndex;

            // TargetID
            if (NeedsTargetId(conditionType))
            {
                var options = GetTargetIdOptions(conditionType);
                DrawConditionalPopupField(options, "Target ID", rect, targetIdProp);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            }

            // Parameter
            if (NeedsParameter(conditionType))
            {
                var options = GetParameterOptions(conditionType);
                DrawConditionalPopupField(options, "Parameter", rect, parameterProp);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            }

            // Value
            if (NeedsValue(conditionType))
            {
                EditorGUI.PropertyField(rect, valueProp);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2) * 4;
        }

        private void DrawConditionalPopupField(string[] options, string label, Rect rect, SerializedProperty prop)
        {
            if (options != null && options.Length > 0)
            {
                int selectedIndex = Mathf.Max(0, System.Array.IndexOf(options, prop.stringValue));
                int newIndex = EditorGUI.Popup(rect, label, selectedIndex, options);
                prop.stringValue = options[newIndex];
            }
            else
            {
                prop.stringValue = EditorGUI.TextField(rect, label, prop.stringValue);
            }
        }

        private string[] GetTargetIdOptions(EEffectType type)
        {
            switch (type)
            {
                case EEffectType.AddItem:
                case EEffectType.RemoveItem:
                    return System.Enum.GetNames(typeof(EItem));
                case EEffectType.AdvanceEndingProgress:
                case EEffectType.EndingComplete:
                    return System.Enum.GetNames(typeof(EEndingType));
                case EEffectType.WorsenStatus:
                case EEffectType.RecoveryStatus:
                case EEffectType.CharacterDaed:
                    return System.Enum.GetNames(typeof(ECharacter));
                default:
                    return null;
            }
        }

        private string[] GetParameterOptions(EEffectType type)
        {
            switch (type)
            {
                case EEffectType.WorsenStatus:
                case EEffectType.RecoveryStatus:
                    return System.Enum.GetNames(typeof(EStatusType));
                default:
                    return null;
            }
        }

        private bool NeedsTargetId(EEffectType type)
        {
            switch (type)
            {
                default:
                    return true;
            }
        }

        private bool NeedsParameter(EEffectType type)
        {
            switch (type)
            {
                case EEffectType.WorsenStatus:
                case EEffectType.RecoveryStatus:
                    return true;
                default:
                    return false;
            }
        }

        private bool NeedsValue(EEffectType type)
        {
            switch (type)
            {
                case EEffectType.AddItem:
                case EEffectType.RemoveItem:
                case EEffectType.WorsenStatus:
                case EEffectType.RecoveryStatus:
                    return true;
                default:
                    return false;
            }
        }
    }
}
