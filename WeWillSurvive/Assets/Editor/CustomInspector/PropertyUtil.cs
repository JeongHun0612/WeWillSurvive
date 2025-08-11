#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace WeWillSurvive
{
    public static class PropertyUtil
    {
        // ============
        //  Basic Get/Set (string 또는 실제 타입 지원)
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


        // ============
        //  GUI Helpers
        // ============


        /// <summary>
        /// Int 전용 입력 필드 (prop이 string/int 어느 쪽이어도 지원).
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
        /// Min/Max 두 개의 정수 필드. Min ≤ Max 보장, 범위 클램프, 변경 시에만 저장.
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
        /// 문자열로 백킹된 enum을 드롭다운으로 표시하고, 선택 결과를 enum 이름 문자열로 저장.
        /// (파싱 실패 시 첫 멤버로 표기)
        /// </summary>
        public static void DrawEnumPopupAsString<TEnum>(ref Rect rect, string label, SerializedProperty stringProp)
            where TEnum : struct, Enum
        {
            float h = EditorGUIUtility.singleLineHeight;

            if (stringProp.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(rect, label, "string 프로퍼티가 필요합니다");
                rect.y += h + 2f;
                return;
            }

            if (!Enum.TryParse<TEnum>(stringProp.stringValue, true, out var current))
            {
                var values = (TEnum[])Enum.GetValues(typeof(TEnum));
                current = values.Length > 0 ? values[0] : default;
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
        /// 문자열로 백킹된 [Flags] enum을 체크박스 플래그로 표시/저장.
        /// 내부적으로 ulong 마스킹으로 정의되지 않은 비트를 제거.
        /// </summary>
        public static void DrawFlagsAsIntString<TEnum>(ref Rect rect, string label, SerializedProperty stringProp)
            where TEnum : struct, Enum
        {
            float h = EditorGUIUtility.singleLineHeight;

            if (stringProp.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(rect, label, "string 프로퍼티가 필요합니다");
                rect.y += h + 2f;
                return;
            }

            if (!typeof(TEnum).IsDefined(typeof(FlagsAttribute), false))
            {
                EditorGUI.LabelField(rect, label, "[Flags] enum이 필요합니다");
                rect.y += h + 2f;
                return;
            }

            // 현재 값 파싱
            ulong current = 0;
            if (!ulong.TryParse(stringProp.stringValue, out current))
                current = 0;

            // 정의된 비트 마스크 생성
            ulong definedMask = 0;
            var values = (TEnum[])Enum.GetValues(typeof(TEnum));
            for (int i = 0; i < values.Length; i++)
                definedMask |= Convert.ToUInt64(values[i]);

            // 유효하지 않은 비트 제거
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
    }
}
#endif