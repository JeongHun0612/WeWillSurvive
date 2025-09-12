using UnityEditor;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Ending;
using WeWillSurvive.GameEvent;
using WeWillSurvive.Item;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Status;

namespace WeWillSurvive
{
    [CustomPropertyDrawer(typeof(EventAction))]
    public class ActionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            var actionTypeProp = property.FindPropertyRelative("_actionType");
            var targetIdProp = property.FindPropertyRelative("_targetId");
            var parameterProp = property.FindPropertyRelative("_parameter");
            var valueProp = property.FindPropertyRelative("_value");

            // EffectType
            PropertyUtil.TryValidateAndFixEnumProperty<EActionType>(actionTypeProp);

            var currentValue = (EActionType)actionTypeProp.intValue;
            var newValue = (EActionType)EditorGUI.EnumPopup(rect, "결과 액션 타입", currentValue);
            if (newValue != currentValue)
                actionTypeProp.intValue = (int)newValue;

            rect.y += EditorGUIUtility.singleLineHeight + 2;
            var actionType = (EActionType)actionTypeProp.intValue;

            switch (actionType)
            {
                case EActionType.WorsenStatus:
                case EActionType.RecoveryStatus:
                    PropertyUtil.DrawEnumPopupAsString<ECharacter>(ref rect, "캐릭터", targetIdProp);
                    PropertyUtil.DrawEnumPopupAsString<EStatusType>(ref rect, "Status", parameterProp);
                    PropertyUtil.DrawIntField(ref rect, "단계", valueProp);
                    break;
                case EActionType.WorsenMaxStatus:
                case EActionType.RecoveryMaxStatus:
                    PropertyUtil.DrawEnumPopupAsString<ECharacter>(ref rect, "캐릭터", targetIdProp);
                    PropertyUtil.DrawEnumPopupAsString<EStatusType>(ref rect, "Status", parameterProp);
                    valueProp.stringValue = string.Empty;
                    break;
                case EActionType.CharacterEventRateModifier:
                    PropertyUtil.DrawEnumPopupAsString<ECharacter>(ref rect, "캐릭터", targetIdProp);
                    parameterProp.stringValue = string.Empty;
                    PropertyUtil.DrawFloatField(ref rect, "보정 값", valueProp);
                    break;
                case EActionType.CharacterDaed:
                    PropertyUtil.DrawEnumPopupAsString<ECharacter>(ref rect, "캐릭터", targetIdProp);
                    parameterProp.stringValue = string.Empty;
                    valueProp.stringValue = string.Empty;
                    break;
                case EActionType.WorsenRandomCharacterStatus:
                case EActionType.WorsenPrioritizedCharacterStatus:
                    targetIdProp.stringValue = string.Empty;
                    PropertyUtil.DrawEnumPopupAsString<EStatusType>(ref rect, "Status", parameterProp);
                    valueProp.stringValue = string.Empty;
                    break;
                case EActionType.AddItem:
                case EActionType.RemoveItem:
                    PropertyUtil.DrawEnumPopupAsString<EItem>(ref rect, "아이템", targetIdProp);
                    parameterProp.stringValue = string.Empty;
                    PropertyUtil.DrawFloatField(ref rect, "갯수", valueProp);
                    break;
                case EActionType.RemoveGreaterOfFoodAndWater:
                    targetIdProp.stringValue = string.Empty;
                    parameterProp.stringValue = string.Empty;
                    PropertyUtil.DrawFloatField(ref rect, "갯수", valueProp);
                    break;
                case EActionType.AdvanceEndingProgress:
                case EActionType.EndingComplete:
                    PropertyUtil.DrawEnumPopupAsString<EEndingType>(ref rect, "엔딩", targetIdProp);
                    parameterProp.stringValue = string.Empty;
                    valueProp.stringValue = string.Empty;
                    break;
                case EActionType.SetSpecificMainEventCooldown:
                case EActionType.AddDaysToSpecificMainEventCooldown:
                    PropertyUtil.DrawEnumPopupAsString<EMainEventCategory>(ref rect, "이벤트 타입", targetIdProp);
                    parameterProp.stringValue = string.Empty;
                    PropertyUtil.DrawIntField(ref rect, "일수", valueProp);
                    break;
                case EActionType.ActivateBuff:
                    PropertyUtil.DrawEnumPopupAsString<EBuffEffect>(ref rect, "버프 타입", targetIdProp);
                    parameterProp.stringValue = string.Empty;
                    PropertyUtil.DrawIntField(ref rect, "지속 날짜", valueProp);
                    break;
                case EActionType.ActivateBuffUntilNextCharacterEvent:
                    PropertyUtil.DrawEnumPopupAsString<EBuffEffect>(ref rect, "버프 타입", targetIdProp);
                    parameterProp.stringValue = string.Empty;
                    valueProp.stringValue = string.Empty;
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
