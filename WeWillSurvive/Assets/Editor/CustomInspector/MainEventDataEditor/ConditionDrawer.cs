using UnityEditor;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Item;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Status;
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
            var value1Prop = property.FindPropertyRelative("value1");
            var value2Prop = property.FindPropertyRelative("value2");

            // ConditionType
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

            // Value 1
            if (NeedsValue1(conditionType))
            {
                EditorGUI.PropertyField(rect, value1Prop);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            }

            // Value 2
            if (NeedsValue2(conditionType))
            {
                EditorGUI.PropertyField(rect, value2Prop);
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
                case EConditionType.CharacterHasStatus:
                case EConditionType.CharacterNotHasStatus:
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
                case EConditionType.CharacterHasStatus:
                case EConditionType.CharacterNotHasStatus:
                    return EnumUtil.GetEnumDescriptions<EStatusType>();
                default:
                    return null;
            }
        }

        private bool NeedsTargetId(EConditionType type)
        {
            switch (type)
            {
                case EConditionType.AliveCount:
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
                case EConditionType.CharacterHasStatus:
                case EConditionType.CharacterNotHasStatus:
                    return true;
                default:
                    return false;
            }
        }
        private bool NeedsValue1(EConditionType type)
        {
            switch (type)
            {
                case EConditionType.AliveCount:
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
        private bool NeedsValue2(EConditionType type)
        {
            switch (type)
            {
                case EConditionType.AliveCount:
                    return true;
                default:
                    return false;
            }
        }
    }
}
