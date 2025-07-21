using UnityEditor;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Item;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Util;

namespace WeWillSurvive
{
    [CustomPropertyDrawer(typeof(Condition))]
    public class ConditionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            var conditionTypeProp = property.FindPropertyRelative("conditionType");
            var targetIdProp = property.FindPropertyRelative("targetId");
            var parameterProp = property.FindPropertyRelative("parameter");
            var valueProp = property.FindPropertyRelative("value");

            // ConditionType
            //var descriptions = EnumUtil.GetEnumDescriptions<EConditionType>();
            //conditionTypeProp.enumValueIndex = EditorGUI.Popup(rect, "이벤트 발생 조건 타입", conditionTypeProp.enumValueIndex, descriptions);
            //rect.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(rect, conditionTypeProp, new GUIContent("이벤트 발생 조건 타입"));
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            var conditionType = (EConditionType)conditionTypeProp.enumValueIndex;

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

        private string[] GetTargetIdOptions(EConditionType type)
        {
            switch (type)
            {
                case EConditionType.CharacterInShelter:
                case EConditionType.CharacterHasState:
                case EConditionType.CharacterNotHasState:
                case EConditionType.CharacterExpeditionCountUpper:
                case EConditionType.CharacterExpeditionCountLower:
                    return System.Enum.GetNames(typeof(ECharacter));
                case EConditionType.ItemCountUpper:
                case EConditionType.ItemCountLower:
                case EConditionType.HasItem:
                    return System.Enum.GetNames(typeof(EItem));
                default:
                    return null;
            }
        }

        private string[] GetParameterOptions(EConditionType type)
        {
            switch (type)
            {
                case EConditionType.CharacterHasState:
                case EConditionType.CharacterNotHasState:
                    return EnumUtil.GetEnumDescriptions<EState>();
                default:
                    return null;
            }
        }

        private bool NeedsTargetId(EConditionType type)
        {
            switch (type)
            {
                case EConditionType.DayCountUpper:
                    return false;
                default:
                    return true;
            }
        }

        private bool NeedsParameter(EConditionType type)
        {
            switch (type)
            {
                case EConditionType.CharacterHasState:
                case EConditionType.CharacterNotHasState:
                    return true;
                default:
                    return false;
            }
        }
        private bool NeedsValue(EConditionType type)
        {
            switch (type)
            {
                case EConditionType.CharacterExpeditionCountUpper:
                case EConditionType.CharacterExpeditionCountLower:
                case EConditionType.ItemCountUpper:
                case EConditionType.ItemCountLower:
                case EConditionType.DayCountUpper:
                    return true;
                default:
                    return false;
            }
        }
    }
}
