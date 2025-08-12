using System;
using UnityEditor;
using UnityEngine;

namespace WeWillSurvive
{
    [CustomEditor(typeof(StatusDebugData))]
    public class StatusDebugDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // 편집하려는 StatusDebugData 객체를 가져옵니다.
            StatusDebugData data = (StatusDebugData)target;

            // Undo/Redo 기능을 위해 변경 사항을 기록합니다.
            Undo.RecordObject(data, "Change Status Debug Data");

            // 각 상태에 대해 커스텀 선택 UI를 그립니다.
            data.Hunger = DrawStatusSelector("허기", data.Hunger);
            data.Thirst = DrawStatusSelector("갈증", data.Thirst);
            data.Injury = DrawStatusSelector("부상/질병", data.Injury);
            data.Anxiety = DrawStatusSelector("불안/공황", data.Anxiety);

            // 변경된 사항이 있다면 디스크에 저장하도록 표시합니다.
            if (GUI.changed)
            {
                EditorUtility.SetDirty(data);
            }
        }

        /// <summary>
        /// 특정 Enum 타입에 대한 라디오 버튼 스타일의 선택 UI를 그려주는 헬퍼 함수입니다.
        /// </summary>
        /// <typeparam name="T">처리할 Enum의 타입</typeparam>
        /// <param name="title">인스펙터에 표시될 제목</param>
        /// <param name="currentValue">현재 선택된 Enum 값</param>
        /// <returns>새롭게 선택된 Enum 값</returns>
        private T DrawStatusSelector<T>(string title, T currentValue) where T : Enum
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            // 최종적으로 반환될 값을 현재 값으로 초기화합니다.
            T newValue = currentValue;

            // Enum의 모든 멤버를 순회합니다.
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                // 현재 순회중인 멤버가 선택된 상태인지 확인합니다.
                bool isSelected = currentValue.Equals(value);

                // 토글을 그리고, 그 결과를 isChecked 변수에 저장합니다.
                bool isChecked = EditorGUILayout.ToggleLeft(value.ToString(), isSelected);

                // 만약 이 토글이 '선택된 상태(isChecked)'이고, '원래는 선택되지 않았었다면(!isSelected)'
                if (isChecked && !isSelected)
                {
                    // 새로운 값으로 확정합니다.
                    newValue = value;
                }
            }

            // 새롭게 선택된 값을 반환합니다.
            return newValue;
        }
    }
}
