using UnityEditor;
using UnityEngine;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    [CustomPropertyDrawer(typeof(ItemData))]
    public class ItemDataDrawer : PropertyDrawer
    {
        // �ν����Ϳ��� ������Ƽ�� ���̸� ����ϴ� �޼ҵ��Դϴ�.
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // �ʿ��� ������Ƽ���� ã���ϴ�.
            SerializedProperty itemProperty = property.FindPropertyRelative("item");
            SerializedProperty isActiveProperty = property.FindPropertyRelative("isActive");

            // EItem enum ���� �����ɴϴ�.
            EItem itemEnum = (EItem)itemProperty.enumValueIndex;

            // �⺻ ���̴� 'isActive' üũ�ڽ��� ���� �� ���Դϴ�.
            float height = EditorGUIUtility.singleLineHeight;

            // ���� �������� Food �Ǵ� Water�̰�, Ȱ��ȭ ���¶�� 'count' �ʵ带 ���� ������ �߰��մϴ�.
            if ((itemEnum == EItem.Food || itemEnum == EItem.Water) && isActiveProperty.boolValue)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        // �ν����Ϳ��� ������Ƽ�� �׸��� �޼ҵ��Դϴ�.
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // �ʿ��� ������Ƽ���� ã���ϴ�.
            SerializedProperty itemProperty = property.FindPropertyRelative("item");
            SerializedProperty isActiveProperty = property.FindPropertyRelative("isActive");
            SerializedProperty countProperty = property.FindPropertyRelative("count");

            // EItem enum ���� �����ɴϴ�.
            EItem itemEnum = (EItem)itemProperty.enumValueIndex;

            EditorGUI.BeginProperty(position, label, property);

            // ù ��° �ٿ� 'isActive' üũ�ڽ��� �׸��ϴ�.
            // ���̺��� ������Ƽ�� �̸�("Food", "Water" ��)�� ����մϴ�.
            Rect lineOneRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(lineOneRect, isActiveProperty, label);

            // �������� Food �Ǵ� Water�� ��쿡�� 'count' �ʵ带 �׸��ϴ�.
            if (itemEnum == EItem.Food || itemEnum == EItem.Water)
            {
                // 'isActive'�� true�� ���� 'count' �ʵ带 �����־� UI�� ����ϰ� �����մϴ�.
                if (isActiveProperty.boolValue)
                {
                    // �� ��° ���� ��ġ�� ����մϴ�.
                    Rect lineTwoRect = new Rect(
                        position.x,
                        position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                        position.width,
                        EditorGUIUtility.singleLineHeight
                    );

                    // 'count' �ʵ带 �鿩�����Ͽ� ���� ������ ��Ȯ�� �����ݴϴ�.
                    EditorGUI.indentLevel++;
                    EditorGUI.PropertyField(lineTwoRect, countProperty, new GUIContent("Count"));
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                // Food, Water�� �ƴ� ������(��: MedicalKit)�� ���,
                // 'count' ���� Ȱ��ȭ ���ο� ���� 1 �Ǵ� 0���� �ڵ� �����մϴ�.
                countProperty.floatValue = isActiveProperty.boolValue ? 1f : 0f;
            }

            EditorGUI.EndProperty();
        }
    }
}
