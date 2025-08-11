using UnityEditor;
using UnityEngine;
using WeWillSurvive.Character;
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

            var actionTypeProp = property.FindPropertyRelative("actionType");
            var targetIdProp = property.FindPropertyRelative("targetId");
            var parameterProp = property.FindPropertyRelative("parameter");
            var valueProp = property.FindPropertyRelative("value");

            // EffectType
            EditorGUI.PropertyField(rect, actionTypeProp, new GUIContent("결과 액션 타입"));
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            var actionType = (EActionType)actionTypeProp.enumValueIndex;

            switch (actionType)
            {
                case EActionType.AddItem:
                case EActionType.RemoveItem:
                    PropertyUtil.DrawEnumPopupAsString<EItem>(ref rect, "아이템", targetIdProp);
                    PropertyUtil.DrawIntField(ref rect, "갯수", valueProp);
                    break;
                case EActionType.AdvanceEndingProgress:
                case EActionType.EndingComplete:
                    PropertyUtil.DrawEnumPopupAsString<EEndingType>(ref rect, "엔딩", targetIdProp);
                    break;
                case EActionType.WorsenStatus:
                case EActionType.RecoveryStatus:
                    PropertyUtil.DrawEnumPopupAsString<ECharacter>(ref rect, "캐릭터", targetIdProp);
                    PropertyUtil.DrawEnumPopupAsString<EStatusType>(ref rect, "Status", parameterProp);
                    PropertyUtil.DrawIntField(ref rect, "단계", valueProp);
                    break;
                case EActionType.CharacterDaed:
                    PropertyUtil.DrawEnumPopupAsString<ECharacter>(ref rect, "캐릭터", targetIdProp);
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
