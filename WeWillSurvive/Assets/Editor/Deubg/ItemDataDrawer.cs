using UnityEditor;
using UnityEngine;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    [CustomPropertyDrawer(typeof(ItemData))]
    public class ItemDataDrawer : PropertyDrawer
    {
        // 인스펙터에서 프로퍼티의 높이를 계산하는 메소드입니다.
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 필요한 프로퍼티들을 찾습니다.
            SerializedProperty itemProperty = property.FindPropertyRelative("item");
            SerializedProperty isActiveProperty = property.FindPropertyRelative("isActive");

            // EItem enum 값을 가져옵니다.
            EItem itemEnum = (EItem)itemProperty.enumValueIndex;

            // 기본 높이는 'isActive' 체크박스를 위한 한 줄입니다.
            float height = EditorGUIUtility.singleLineHeight;

            // 만약 아이템이 Food 또는 Water이고, 활성화 상태라면 'count' 필드를 위한 공간을 추가합니다.
            if ((itemEnum == EItem.Food || itemEnum == EItem.Water) && isActiveProperty.boolValue)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        // 인스펙터에서 프로퍼티를 그리는 메소드입니다.
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 필요한 프로퍼티들을 찾습니다.
            SerializedProperty itemProperty = property.FindPropertyRelative("item");
            SerializedProperty isActiveProperty = property.FindPropertyRelative("isActive");
            SerializedProperty countProperty = property.FindPropertyRelative("count");

            // EItem enum 값을 가져옵니다.
            EItem itemEnum = (EItem)itemProperty.enumValueIndex;

            EditorGUI.BeginProperty(position, label, property);

            // 첫 번째 줄에 'isActive' 체크박스를 그립니다.
            // 레이블은 프로퍼티의 이름("Food", "Water" 등)을 사용합니다.
            Rect lineOneRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(lineOneRect, isActiveProperty, label);

            // 아이템이 Food 또는 Water인 경우에만 'count' 필드를 그립니다.
            if (itemEnum == EItem.Food || itemEnum == EItem.Water)
            {
                // 'isActive'가 true일 때만 'count' 필드를 보여주어 UI를 깔끔하게 유지합니다.
                if (isActiveProperty.boolValue)
                {
                    // 두 번째 줄의 위치를 계산합니다.
                    Rect lineTwoRect = new Rect(
                        position.x,
                        position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                        position.width,
                        EditorGUIUtility.singleLineHeight
                    );

                    // 'count' 필드를 들여쓰기하여 계층 구조를 명확히 보여줍니다.
                    EditorGUI.indentLevel++;
                    EditorGUI.PropertyField(lineTwoRect, countProperty, new GUIContent("Count"));
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                // Food, Water가 아닌 아이템(예: MedicalKit)의 경우,
                // 'count' 값을 활성화 여부에 따라 1 또는 0으로 자동 설정합니다.
                countProperty.floatValue = isActiveProperty.boolValue ? 1f : 0f;
            }

            EditorGUI.EndProperty();
        }
    }
}
