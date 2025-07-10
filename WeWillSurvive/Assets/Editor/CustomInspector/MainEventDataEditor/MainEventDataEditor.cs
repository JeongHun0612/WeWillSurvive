using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WeWillSurvive.MainEvent;

namespace WeWillSurvive
{
    #region EIconType
    public enum EIconType
    {
        // O, X 아이콘
        O = 0,
        X,

        // 캐릭터 아이콘
        Lead = 100,
        Cook,
        Bell,
        DrK,

        // 아이템 아이콘
        Food = 200,
        SpecialFood,
        Water,
        MedicKit,
        SuperMedicKit,
        RepairKit,
        SuperRepairKit,
        NiceSpacesuit,
        Radio,
        LaserGun,
        BoardGame,

        // 선택하지 않음
        Noting = 300,
    }
    #endregion

    [CustomEditor(typeof(MainEventData))]
    public class MainEventDataEditor : Editor
    {
        private Dictionary<EIconType, Texture2D> _iconTextures = new();

        private int selectedChoiceIndex = 0;
        private int selectedResultIndex = 0;

        private void OnEnable()
        {
            LoadIconTextures();

            selectedChoiceIndex = 0;
            selectedResultIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var eventIdProp = serializedObject.FindProperty("eventId");
            var titleProp = serializedObject.FindProperty("title");
            var descriptionsProp = serializedObject.FindProperty("descriptions");
            var triggerConditionsProp = serializedObject.FindProperty("triggerConditions");
            var eventTypeProp = serializedObject.FindProperty("eventType");

            MainEventData data = (MainEventData)target;

            // 기본 필드 출력
            EditorGUILayout.PropertyField(eventIdProp, new GUIContent("이벤트 ID"));
            EditorGUILayout.PropertyField(titleProp, new GUIContent("타이틀"));
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(descriptionsProp, new GUIContent("이벤트 내용 목록"));
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(triggerConditionsProp, new GUIContent("이벤트 발생 조건 목록"));

            // 이벤트 타입 선택
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("이벤트 타입", EditorStyles.boldLabel, GUILayout.Width(100));
            var options = Util.EnumUtil.GetEnumDescriptions<EMainEventType>();
            int newType = EditorGUILayout.Popup(eventTypeProp.enumValueIndex, options, GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();

            // 이벤트 타입이 변경되면 choices 초기화
            if (newType != eventTypeProp.enumValueIndex || data.choices == null)
            {
                eventTypeProp.enumValueIndex = newType;
                serializedObject.ApplyModifiedProperties(); // 먼저 반영

                InitializeChoicesForEventType((EMainEventType)newType, data);
                EditorUtility.SetDirty(data);

                serializedObject.Update(); // 변경 반영한 뒤 다시 동기화
            }

            // 이벤트 타입 별 UI 변경
            EditorGUILayout.Space(20);
            SerializedProperty choicesProp = serializedObject.FindProperty("choices");
            var eventType = (EMainEventType)eventTypeProp.enumValueIndex;
            switch (eventType)
            {
                case EMainEventType.YesOrNo:
                case EMainEventType.SendSomeone:
                    DrawStaticChoiceUI(choicesProp);
                    break;
                case EMainEventType.UseItems:
                    DrawEditableChoiceUI(choicesProp, GetItemIconNames());
                    break;
                case EMainEventType.ChooseSomeone:
                    DrawEditableChoiceUI(choicesProp, GetCharacterIconNames());
                    break;
                case EMainEventType.Trade:
                    DrawEditableChoiceUI(choicesProp, GetItemIconNames());
                    break;
                default:
                    EditorGUILayout.PropertyField(choicesProp);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStaticChoiceUI(SerializedProperty choicesProp)
        {
            EditorGUILayout.LabelField("선택지 목록", EditorStyles.boldLabel, GUILayout.Width(80));

            DrawChoiceSelector(choicesProp);
            EditorGUILayout.Space(10);

            // 선택한 선택지 결과 출력
            SerializedProperty selectedChoiceProp = choicesProp.GetArrayElementAtIndex(selectedChoiceIndex);
            SerializedProperty resultsProp = selectedChoiceProp.FindPropertyRelative("results");
            DrawResultTabsUI(resultsProp);
        }

        private void DrawEditableChoiceUI(SerializedProperty choicesProp, string[] choiceOptions)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("선택지 목록", EditorStyles.boldLabel, GUILayout.Width(80));
            DrawAddRemoveButtonUI(choicesProp, ref selectedChoiceIndex, InitializeEventChoiceProperty);
            EditorGUILayout.EndHorizontal();

            DrawChoiceSelector(choicesProp);
            EditorGUILayout.Space(10);

            // 선택지 편집 영역
            selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, choicesProp.arraySize - 1);

            if (choicesProp.arraySize > 0)
            {
                SerializedProperty selectedChoiceProp = choicesProp.GetArrayElementAtIndex(selectedChoiceIndex);
                SerializedProperty choiceIdProp = selectedChoiceProp.FindPropertyRelative("choiceId");
                SerializedProperty resultsProp = selectedChoiceProp.FindPropertyRelative("results");

                // EIconType 드롭다운으로 선택
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("선택지 타입", EditorStyles.boldLabel, GUILayout.Width(100));
                int currentIndex = Mathf.Max(0, Array.IndexOf(choiceOptions, choiceIdProp.stringValue));
                //int newType = EditorGUILayout.Popup(eventTypeProp.enumValueIndex, options, GUILayout.Width(300));
                int newIndex = EditorGUILayout.Popup(currentIndex, choiceOptions, GUILayout.Width(300));
                choiceIdProp.stringValue = choiceOptions[newIndex];
                EditorGUILayout.EndHorizontal();

                // EventResult 출력
                EditorGUILayout.Space(20);
                DrawResultTabsUI(resultsProp);
            }
        }

        private void DrawChoiceSelector(SerializedProperty choicesProp)
        {
            if (choicesProp == null)
                return;

            if (choicesProp.arraySize == 0)
                EditorGUILayout.HelpBox("선택지를 추가해주세요.", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < choicesProp.arraySize; i++)
            {
                SerializedProperty choiceProp = choicesProp.GetArrayElementAtIndex(i);
                string choiceId = choiceProp.FindPropertyRelative("choiceId").stringValue;

                Texture2D icon = null;
                if (Enum.TryParse(choiceId, out EIconType iconType))
                    icon = GetIconTexture(iconType);

                GUIStyle style = new GUIStyle(GUI.skin.button)
                {
                    fixedWidth = 64,
                    fixedHeight = 64,
                    margin = new RectOffset(10, 10, 5, 5),
                    padding = new RectOffset(2, 2, 2, 2)
                };

                Rect buttonRect = GUILayoutUtility.GetRect(64, 64, style);

                if (selectedChoiceIndex == i)
                {
                    EditorGUI.DrawRect(new Rect(buttonRect.x - 2, buttonRect.y - 2, 68, 68), Color.cyan);
                }

                GUIContent content = icon != null ? new GUIContent(icon) : new GUIContent(choiceId);
                if (GUI.Button(buttonRect, content, style))
                {
                    selectedChoiceIndex = i;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawResultTabsUI(SerializedProperty resultsProp)
        {
            if (resultsProp == null) return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("결과 목록", EditorStyles.boldLabel, GUILayout.Width(80));
            DrawAddRemoveButtonUI(resultsProp, ref selectedResultIndex, InitializeEventResultProperty);
            EditorGUILayout.EndHorizontal();

            // 탭 버튼 영역
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < resultsProp.arraySize; i++)
            {
                string tabName = $"결과 {i + 1}";
                if (GUILayout.Toggle(selectedResultIndex == i, tabName, "Button", GUILayout.MaxWidth(100)))
                {
                    selectedResultIndex = i;
                }
            }
            EditorGUILayout.EndHorizontal();

            // 선택된 결과 출력
            EditorGUILayout.Space(5);
            selectedResultIndex = Mathf.Clamp(selectedResultIndex, 0, resultsProp.arraySize - 1);

            if (resultsProp.arraySize > 0)
            {
                SerializedProperty selectedResult = resultsProp.GetArrayElementAtIndex(selectedResultIndex);
                if (selectedResult == null) return;

                SerializedProperty conditionsProp = selectedResult.FindPropertyRelative("conditions");
                SerializedProperty textProp = selectedResult.FindPropertyRelative("resultText");
                SerializedProperty effectsProp = selectedResult.FindPropertyRelative("effects");
                SerializedProperty probProp = selectedResult.FindPropertyRelative("probability");

                // 결과 데이터 필드
                EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(30, 30, 20, 20),
                    margin = new RectOffset(20, 20, 10, 10)
                });

                EditorGUILayout.PropertyField(conditionsProp, new GUIContent("발생 조건"), true);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(textProp, new GUIContent("결과 텍스트"));
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(effectsProp, new GUIContent("효과"), true);
                EditorGUILayout.Space(5);
                EditorGUILayout.Slider(probProp, 0f, 1f, new GUIContent("발생 확률"));
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawAddRemoveButtonUI(SerializedProperty prop, ref int selectedIndex, Action<SerializedProperty> onElementInserted = null)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(24)))
            {
                int insertIndex = prop.arraySize;
                prop.InsertArrayElementAtIndex(insertIndex);
                selectedIndex = insertIndex;

                SerializedProperty newElement = prop.GetArrayElementAtIndex(insertIndex);
                onElementInserted?.Invoke(newElement);
            }

            GUI.enabled = prop.arraySize > 0;
            if (GUILayout.Button("-", GUILayout.Width(24)))
            {
                prop.DeleteArrayElementAtIndex(selectedIndex);
                selectedIndex = Mathf.Clamp(selectedIndex - 1, 0, prop.arraySize - 1);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        private void InitializeChoicesForEventType(EMainEventType type, MainEventData data)
        {
            selectedChoiceIndex = 0;
            selectedResultIndex = 0;

            switch (type)
            {
                case EMainEventType.YesOrNo:
                    data.choices = new List<EventChoice>
                    {
                        new EventChoice { choiceId = EIconType.O.ToString(), results = new List<EventResult>() { new EventResult() }},
                        new EventChoice { choiceId = EIconType.X.ToString(), results = new List<EventResult>() { new EventResult() }},
                    };
                    break;
                case EMainEventType.SendSomeone:
                    data.choices = new List<EventChoice>
                    {
                        new EventChoice { choiceId = "누군가를\n보냄", results = new List<EventResult>() { new EventResult() }},
                        new EventChoice { choiceId = EIconType.Noting.ToString(), results = new List<EventResult>() { new EventResult() }},
                    };
                    break;
                case EMainEventType.ChooseSomeone:
                case EMainEventType.UseItems:
                case EMainEventType.Trade:
                    data.choices = new List<EventChoice>(); // 초기 빈 리스트 (에디터에서 추가)
                    break;

            }
        }

        private void InitializeEventChoiceProperty(SerializedProperty choiceProp)
        {
            if (choiceProp == null) return;

            SerializedProperty resultsProp = choiceProp.FindPropertyRelative("results");
            resultsProp.ClearArray();
            resultsProp.arraySize = 1;

            SerializedProperty result = resultsProp.GetArrayElementAtIndex(0);
            InitializeEventResultProperty(result);
        }

        private void InitializeEventResultProperty(SerializedProperty resultProp)
        {
            if (resultProp == null) return;

            SerializedProperty textProp = resultProp.FindPropertyRelative("resultText");
            SerializedProperty probProp = resultProp.FindPropertyRelative("probability");
            SerializedProperty conditionsProp = resultProp.FindPropertyRelative("conditions");
            SerializedProperty effectsProp = resultProp.FindPropertyRelative("effects");

            textProp.stringValue = string.Empty;
            probProp.floatValue = 1.0f;

            conditionsProp.ClearArray();
            effectsProp.ClearArray();
        }

        private void LoadIconTextures()
        {
            _iconTextures.Clear();

            foreach (EIconType type in Enum.GetValues(typeof(EIconType)))
            {
                string path = $"Assets/Editor/Icons/{type}.png"; // enum 이름 그대로 사용
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                if (texture != null)
                    _iconTextures[type] = texture;
            }
        }

        private Texture2D GetIconTexture(EIconType type)
        {
            if (_iconTextures.TryGetValue(type, out var texture))
                return texture;

            return null;
        }

        private string[] GetAnswerIconNames() => GetIconNamesInRange(0, 99);
        private string[] GetCharacterIconNames() => GetIconNamesInRange(100, 199);
        private string[] GetItemIconNames() => GetIconNamesInRange(200, 299);

        private string[] GetIconNamesInRange(int minInclusive, int maxInclusive)
        {
            return Enum.GetValues(typeof(EIconType))
                .Cast<EIconType>()
                .Where(e => (int)e >= minInclusive && (int)e <= maxInclusive)
                .Select(e => e.ToString())
                .ToArray();
        }
    }
}
