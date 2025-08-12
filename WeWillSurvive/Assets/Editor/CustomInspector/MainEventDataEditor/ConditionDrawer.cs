using System;
using UnityEditor;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Item;
using WeWillSurvive.MainEvent;

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
            PropertyUtil.TryValidateAndFixEnumProperty<EConditionType>(conditionTypeProp);

            var currentValue = (EConditionType)conditionTypeProp.intValue;
            var newValue = (EConditionType)EditorGUI.EnumPopup(rect, "이벤트 발생 조건 타입", currentValue);
            if (newValue != currentValue)
                conditionTypeProp.intValue = (int)newValue;

            rect.y += EditorGUIUtility.singleLineHeight + 2;
            var conditionType = (EConditionType)conditionTypeProp.intValue;

            switch (conditionType)
            {
                case EConditionType.CharacterInShelter:
                    PropertyUtil.DrawEnumPopupAsString<ECharacter>(ref rect, "캐릭터", targetIdProp);
                    break;
                case EConditionType.CharacterHasState:
                case EConditionType.CharacterNotHasState:
                    PropertyUtil.DrawEnumPopupAsString<ECharacter>(ref rect, "캐릭터", targetIdProp);
                    PropertyUtil.DrawFlagsAsIntString<EState>(ref rect, "상태", parameterProp);
                    break;
                case EConditionType.CharacterExpeditionCountUpper:
                case EConditionType.CharacterExpeditionCountLower:
                    PropertyUtil.DrawEnumPopupAsString<ECharacter>(ref rect, "캐릭터", targetIdProp);
                    PropertyUtil.DrawIntField(ref rect, "탐사 횟수", value1Prop);
                    break;
                case EConditionType.HasItem:
                    PropertyUtil.DrawEnumPopupAsString<EItem>(ref rect, "아이템", targetIdProp);
                    break;
                case EConditionType.ItemCountUpper:
                case EConditionType.ItemCountLower:
                    PropertyUtil.DrawEnumPopupAsString<EItem>(ref rect, "아이템", targetIdProp);
                    PropertyUtil.DrawIntField(ref rect, "갯수", value1Prop);
                    break;
                case EConditionType.AliveCount:
                    PropertyUtil.DrawMinMaxIntFields(ref rect, "최소 인원", value1Prop, "최대 인원", value2Prop, 0, 4);
                    break;
                case EConditionType.TotalExpeditionCountUpper:
                    PropertyUtil.DrawIntField(ref rect, "탐사 횟수", value1Prop);
                    break;
                case EConditionType.DayCountUpper:
                    PropertyUtil.DrawIntField(ref rect, "날짜", value1Prop);
                    break;
                default:
                    break;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2) * 4;
        }
    }
}
