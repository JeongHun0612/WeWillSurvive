using UnityEditor;
using UnityEngine;

namespace WeWillSurvive
{
    [CustomEditor(typeof(RoomBase))]
    public class RoomBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty roomType = serializedObject.FindProperty("_roomType");
            SerializedProperty background = serializedObject.FindProperty("_background");
            EditorGUILayout.PropertyField(roomType);
            EditorGUILayout.PropertyField(background);

            if (serializedObject.targetObject.GetType() == typeof(RoomBase))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_characterUI"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_lightOff"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
