#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace WeWillSurvive
{
    public static class PropertyUtil
    {
        // ============
        //  Basic Get/Set (string �Ǵ� ���� Ÿ�� ����)
        // ============


        public static int GetInt(SerializedProperty prop, int fallback = 0)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return prop.intValue;
                case SerializedPropertyType.String:
                    return int.TryParse(prop.stringValue, out var v) ? v : fallback;
                default:
                    return fallback;
            }
        }

        public static void SetInt(SerializedProperty prop, int value)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    if (prop.intValue != value) prop.intValue = value;
                    break;
                case SerializedPropertyType.String:
                    string s = value.ToString();
                    if (prop.stringValue != s) prop.stringValue = s;
                    break;
                default:
                    break;
            }
        }

        public static float GetFloat(SerializedProperty prop, float fallback = 0f)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Float:
                    return prop.floatValue;
                case SerializedPropertyType.String:
                    return float.TryParse(prop.stringValue, out var v) ? v : fallback;
                default:
                    return fallback;
            }
        }

        public static void SetFloat(SerializedProperty prop, float value)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Float:
                    if (prop.floatValue != value) prop.floatValue = value;
                    break;
                case SerializedPropertyType.String:
                    string s = value.ToString();
                    if (prop.stringValue != s) prop.stringValue = s;
                    break;
                default:
                    break;
            }
        }


        // ============
        //  GUI Helpers
        // ============


        /// <summary>
        /// Int ���� �Է� �ʵ� (prop�� string/int ��� ���̾ ����).
        /// </summary>
        public static int DrawIntField(ref Rect rect, string label, SerializedProperty prop, int minLimit = int.MinValue, int maxLimit = int.MaxValue)
        {
            float h = EditorGUIUtility.singleLineHeight;
            int current = GetInt(prop, 0);

            EditorGUI.BeginChangeCheck();
            int newInput = EditorGUI.IntField(rect, label, current);
            rect.y += h + 2f;
            bool changed = EditorGUI.EndChangeCheck();

            if (changed)
            {
                newInput = Mathf.Clamp(newInput, minLimit, maxLimit);
                SetInt(prop, newInput);
                return newInput;
            }

            return current;
        }

        /// <summary>
        /// Float ���� �Է� �ʵ� (prop�� string/float ��� ���̾ ����).
        /// </summary>
        public static float DrawFloatField(ref Rect rect, string label, SerializedProperty prop, float
        minLimit = float.MinValue, float maxLimit = float.MaxValue)
        {
            float h = EditorGUIUtility.singleLineHeight;
            float current = GetFloat(prop, 0f);

            EditorGUI.BeginChangeCheck();
            float newInput = EditorGUI.FloatField(rect, label, current);
            rect.y += h + 2f;
            bool changed = EditorGUI.EndChangeCheck();

            if (changed)
            {
                newInput = Mathf.Clamp(newInput, minLimit, maxLimit);
                SetFloat(prop, newInput);
                return newInput;
            }

            return current;
        }

        /// <summary>
        /// Min/Max �� ���� ���� �ʵ�. Min �� Max ����, ���� Ŭ����, ���� �ÿ��� ����.
        /// </summary>
        public static void DrawMinMaxIntFields(ref Rect rect,
                                               string minLabel, SerializedProperty minProp,
                                               string maxLabel, SerializedProperty maxProp,
                                               int minLimit = int.MinValue, int maxLimit = int.MaxValue)
        {
            float h = EditorGUIUtility.singleLineHeight;

            int minValue = GetInt(minProp, minLimit);
            int maxValue = GetInt(maxProp, maxLimit);

            EditorGUI.BeginChangeCheck();

            int newMin = EditorGUI.IntField(rect, minLabel, minValue);
            rect.y += h + 2f;

            int newMax = EditorGUI.IntField(rect, maxLabel, maxValue);
            rect.y += h + 2f;

            bool changed = EditorGUI.EndChangeCheck();
            if (!changed) return;

            newMin = Mathf.Clamp(newMin, minLimit, maxLimit);
            newMax = Mathf.Clamp(newMax, minLimit, maxLimit);
            if (newMin > newMax) newMax = newMin;

            SetInt(minProp, newMin);
            SetInt(maxProp, newMax);
        }

        /// <summary>
        /// ���ڿ��� ��ŷ�� enum�� ��Ӵٿ����� ǥ���ϰ�, ���� ����� enum �̸� ���ڿ��� ����.
        /// (�Ľ� ���� �� ù ����� ǥ��)
        /// </summary>
        public static void DrawEnumPopupAsString<TEnum>(ref Rect rect, string label, SerializedProperty stringProp)
            where TEnum : struct, Enum
        {
            float h = EditorGUIUtility.singleLineHeight;

            if (stringProp.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(rect, label, "string ������Ƽ�� �ʿ��մϴ�");
                rect.y += h + 2f;
                return;
            }
           
            if (!Enum.TryParse<TEnum>(stringProp.stringValue, true, out var current))
            {
                var values = (TEnum[])Enum.GetValues(typeof(TEnum));
                current = values.Length > 0 ? values[0] : default;
                stringProp.stringValue = current.ToString();
            }

            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.EnumPopup(rect, label, (Enum)(object)current);
            if (EditorGUI.EndChangeCheck())
            {
                stringProp.stringValue = newValue.ToString();
            }

            rect.y += h + 2f;
        }

        /// <summary>
        /// ���ڿ��� ��ŷ�� [Flags] enum�� üũ�ڽ� �÷��׷� ǥ��/����.
        /// ���������� ulong ����ŷ���� ���ǵ��� ���� ��Ʈ�� ����.
        /// </summary>
        public static void DrawFlagsAsIntString<TEnum>(ref Rect rect, string label, SerializedProperty stringProp)
            where TEnum : struct, Enum
        {
            float h = EditorGUIUtility.singleLineHeight;

            if (stringProp.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(rect, label, "string ������Ƽ�� �ʿ��մϴ�");
                rect.y += h + 2f;
                return;
            }

            if (!typeof(TEnum).IsDefined(typeof(FlagsAttribute), false))
            {
                EditorGUI.LabelField(rect, label, "[Flags] enum�� �ʿ��մϴ�");
                rect.y += h + 2f;
                return;
            }

            // ���� �� �Ľ�
            ulong current = 0;
            if (!ulong.TryParse(stringProp.stringValue, out current))
                current = 0;

            // ���ǵ� ��Ʈ ����ũ ����
            ulong definedMask = 0;
            var values = (TEnum[])Enum.GetValues(typeof(TEnum));
            for (int i = 0; i < values.Length; i++)
                definedMask |= Convert.ToUInt64(values[i]);

            // ��ȿ���� ���� ��Ʈ ����
            current &= definedMask;

            // UI
            var boxed = (Enum)Enum.ToObject(typeof(TEnum), current);
            EditorGUI.BeginChangeCheck();
            var picked = EditorGUI.EnumFlagsField(rect, label, boxed);
            if (EditorGUI.EndChangeCheck())
            {
                ulong newVal = Convert.ToUInt64(picked) & definedMask;
                string s = newVal.ToString();
                if (stringProp.stringValue != s)
                    stringProp.stringValue = s;
            }

            rect.y += h + 2f;
        }


        // ============
        // UTil
        // ============

        /// <summary>
        /// SerializedProperty�� ��ȿ�� Enum ���� ������ �ִ��� Ȯ���ϰ�,
        /// �׷��� �ʴٸ� Enum�� ù ��° ������ �����մϴ�.
        /// </summary>
        /// <typeparam name="T">�˻��� Enum�� Ÿ��</typeparam>
        /// <param name="property">�˻� �� ������ SerializedProperty (int ��� Enum�̾�� ��)</param>
        /// <returns>���� �����Ǿ��ٸ� true, �������� ��ȿ�ߴٸ� false�� ��ȯ�մϴ�.</returns>
        public static bool TryValidateAndFixEnumProperty<T>(SerializedProperty property) where T : Enum
        {
            // ������Ƽ�� int Ÿ���� ������� �ϴ��� Ȯ��
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                // �����Ϳ����� Enum�� int�� ��޵� ���� �����Ƿ� int�� ���
                if (property.propertyType != SerializedPropertyType.Integer)
                {
                    Debug.LogError($"{property.name} is not an Enum or Integer property.");
                    return false;
                }
            }

            var enumValue = (T)Enum.ToObject(typeof(T), property.intValue);

            // ���� ���� Enum�� ���ǵǾ� �ִ��� Ȯ��
            if (!Enum.IsDefined(typeof(T), enumValue))
            {
                var allValues = Enum.GetValues(typeof(T));
                if (allValues.Length > 0)
                {
                    // ù ��° ������ ����
                    property.intValue = Convert.ToInt32(allValues.GetValue(0));
                    return true;
                }
            }

            return false;
        }
    }
}
#endif