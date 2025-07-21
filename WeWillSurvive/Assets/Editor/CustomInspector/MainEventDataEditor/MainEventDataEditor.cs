using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WeWillSurvive.MainEvent;
using WeWillSurvive.Util;

namespace WeWillSurvive
{
    [CustomEditor(typeof(MainEventData))]
    public class MainEventDataEditor : Editor
    {
        private const float ICON_FIXED_WIDTH = 64;
        private const float ICON_FIXED_HEIGHT = 64;

        private Dictionary<EChoiceType, Texture2D> _iconTextures = new();

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
            var options = EnumUtil.GetEnumDescriptions<EMainEventType>();
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
            SerializedProperty iconTexturesProp = serializedObject.FindProperty("iconTextures");
            var eventType = (EMainEventType)eventTypeProp.enumValueIndex;
            switch (eventType)
            {
                case EMainEventType.YesOrNo:
                case EMainEventType.SendSomeone:
                case EMainEventType.Noting:
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
            DrawAddRemoveChoiceButtonUI(choicesProp);
            EditorGUILayout.EndHorizontal();

            DrawChoiceSelector(choicesProp);
            EditorGUILayout.Space(10);

            // 선택지 편집 영역
            selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, choicesProp.arraySize - 1);

            if (choicesProp.arraySize > 0)
            {
                SerializedProperty selectedChoiceProp = choicesProp.GetArrayElementAtIndex(selectedChoiceIndex);
                SerializedProperty choiceTypeProp = selectedChoiceProp.FindPropertyRelative("choiceType");
                SerializedProperty iconTextureProp = selectedChoiceProp.FindPropertyRelative("iconTexture");
                SerializedProperty resultsProp = selectedChoiceProp.FindPropertyRelative("results");

                // EIconType 드롭다운으로 선택
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("선택지 타입", EditorStyles.boldLabel, GUILayout.Width(100));

                int enumIndex = choiceTypeProp.enumValueIndex;
                string enumName = choiceTypeProp.enumNames[enumIndex];
                string choiceName = EnumUtil.GetDescription((EChoiceType)Enum.Parse(typeof(EChoiceType), enumName));

                int choiceIndex = Array.IndexOf(choiceOptions, choiceName);
                int currentIndex = Mathf.Max(0, choiceIndex);

                int newIndex = EditorGUILayout.Popup(currentIndex, choiceOptions, GUILayout.Width(300));
                if (currentIndex != newIndex || choiceIndex < 0)
                {
                    EChoiceType choiceType = EnumUtil.GetEnumByDescription<EChoiceType>(choiceOptions[newIndex]).Value;
                    int newEnumValueIndex = EnumUtil.GetEnumIndex(choiceType);

                    choiceTypeProp.enumValueIndex = newEnumValueIndex;
                    iconTextureProp.objectReferenceValue = GetIconTexture(choiceType);
                }
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

            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = ICON_FIXED_WIDTH,
                fixedHeight = ICON_FIXED_HEIGHT,
                margin = new RectOffset(10, 10, 5, 5),
                padding = new RectOffset(2, 2, 2, 2),
                wordWrap = true
            };

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < choicesProp.arraySize; i++)
            {
                SerializedProperty choiceProp = choicesProp.GetArrayElementAtIndex(i);
                SerializedProperty choiceTypeProp = choiceProp.FindPropertyRelative("choiceType");
                SerializedProperty iconTextureProp = choiceProp.FindPropertyRelative("iconTexture");

                int index = choiceTypeProp.enumValueIndex;
                string enumName = choiceTypeProp.enumNames[index];
                EChoiceType choiceType = (EChoiceType)Enum.Parse(typeof(EChoiceType), enumName);

                Rect buttonRect = GUILayoutUtility.GetRect(ICON_FIXED_WIDTH, ICON_FIXED_HEIGHT, style);

                if (selectedChoiceIndex == i)
                {
                    EditorGUI.DrawRect(new Rect(buttonRect.x - 2, buttonRect.y - 2, ICON_FIXED_WIDTH + 4, ICON_FIXED_HEIGHT + 4), Color.cyan);
                }

                Texture2D iconTexture = iconTextureProp.objectReferenceValue as Texture2D;
                GUIContent content = iconTexture != null ? new GUIContent(iconTexture) : new GUIContent(EnumUtil.GetDescription(choiceType));
                if (GUI.Button(buttonRect, content, style))
                {
                    selectedChoiceIndex = i;
                    GUI.FocusControl(null);
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

            if (resultsProp.arraySize == 0)
                EditorGUILayout.HelpBox("결과를 추가해주세요.", MessageType.Info);

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

        private void DrawAddRemoveChoiceButtonUI(SerializedProperty choicesProp)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(24)))
            {
                // Choice 생성 및 초기화
                int choiceInsertIndex = choicesProp.arraySize;
                choicesProp.InsertArrayElementAtIndex(choiceInsertIndex);
                SerializedProperty newChoiceElement = choicesProp.GetArrayElementAtIndex(choiceInsertIndex);
                InitializeEventChoiceProperty(newChoiceElement);

                selectedChoiceIndex = choiceInsertIndex;
            }

            GUI.enabled = choicesProp.arraySize > 0;
            if (GUILayout.Button("-", GUILayout.Width(24)))
            {
                choicesProp.DeleteArrayElementAtIndex(selectedChoiceIndex);
                selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex - 1, 0, choicesProp.arraySize - 1);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
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
                    {
                        data.choices = new List<EventChoice>
                        {
                            new EventChoice
                            {
                                choiceType = EChoiceType.Yes,
                                iconTexture = GetIconTexture(EChoiceType.Yes),
                                results = new List<EventResult>() { new EventResult() }
                            },
                            new EventChoice
                            {
                                choiceType = EChoiceType.No,
                                iconTexture = GetIconTexture(EChoiceType.No),
                                results = new List<EventResult>() { new EventResult() }
                            },
                        };
                    }
                    break;
                case EMainEventType.SendSomeone:
                    {
                        data.choices = new List<EventChoice>
                        {
                            new EventChoice
                            {
                                choiceType = EChoiceType.Lead,
                                iconTexture = GetIconTexture(EChoiceType.Lead),
                                results = new List<EventResult>() { new EventResult() }
                            },
                            new EventChoice
                            {
                                choiceType = EChoiceType.Cook,
                                iconTexture = GetIconTexture(EChoiceType.Cook),
                                results = new List<EventResult>() { new EventResult() }
                            },
                            new EventChoice
                            {
                                choiceType = EChoiceType.Bell,
                                iconTexture = GetIconTexture(EChoiceType.Bell),
                                results = new List<EventResult>() { new EventResult() }
                            },
                            new EventChoice
                            {
                                choiceType = EChoiceType.DrK,
                                iconTexture = GetIconTexture(EChoiceType.DrK),
                                results = new List<EventResult>() { new EventResult() }
                            },
                            new EventChoice
                            {
                                choiceType = EChoiceType.Noting,
                                iconTexture = null,
                                results = new List<EventResult>() { new EventResult() }
                            },
                        };
                    }
                    break;
                case EMainEventType.Noting:
                    {
                        data.choices = new List<EventChoice>
                        {
                            new EventChoice
                            {
                                choiceType = EChoiceType.Noting,
                                iconTexture = null,
                                results = new List<EventResult>() { new EventResult() }
                            }
                        };
                    }
                    break;
                case EMainEventType.ChooseSomeone:
                case EMainEventType.UseItems:
                case EMainEventType.Trade:
                    data.choices = new List<EventChoice>();     // 초기 빈 리스트 (에디터에서 추가)
                    break;

            }
        }

        private void InitializeEventChoiceProperty(SerializedProperty choiceProp)
        {
            if (choiceProp == null) return;

            SerializedProperty choiceTypeProp = choiceProp.FindPropertyRelative("choiceType");
            choiceTypeProp.enumValueIndex = EnumUtil.GetEnumIndex(EChoiceType.None);

            SerializedProperty iconTextureProp = choiceProp.FindPropertyRelative("iconTexture");
            iconTextureProp.objectReferenceValue = null;

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

            foreach (EChoiceType type in Enum.GetValues(typeof(EChoiceType)))
            {
                string path = $"Assets/Editor/Icons/{type}.png"; // enum 이름 그대로 사용
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                if (texture != null)
                    _iconTextures[type] = texture;
            }
        }

        private Texture2D GetIconTexture(EChoiceType type)
        {
            if (_iconTextures.TryGetValue(type, out var texture))
                return texture;

            return null;
        }

        private string[] GetAnswerIconNames() => GetIconNamesInRange(0, 99);
        private string[] GetCharacterIconNames()
        {
            return GetItemChoiceTypes(100, 199)
                .Append(EChoiceType.Noting)
                .Select(e => EnumUtil.GetDescription(e))
                .ToArray();
        }

        private string[] GetItemIconNames()
        {
            return GetItemChoiceTypes(200, 299)
                .Append(EChoiceType.Noting)
                .Select(e => EnumUtil.GetDescription(e))
                .ToArray();
        }

        private string[] GetIconNamesInRange(int minInclusive, int maxInclusive)
        {
            return Enum.GetValues(typeof(EChoiceType))
                .Cast<EChoiceType>()
                .Where(e => (int)e >= minInclusive && (int)e <= maxInclusive)
                .Select(e => e.ToString())
                .ToArray();
        }

        private EChoiceType[] GetItemChoiceTypes(int minInclusive, int maxInclusive)
        {
            return Enum.GetValues(typeof(EChoiceType))
                .Cast<EChoiceType>()
                .Where(e => (int)e >= minInclusive && (int)e < maxInclusive)
                .ToArray();
        }
    }
}
