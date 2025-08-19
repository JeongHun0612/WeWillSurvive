using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using WeWillSurvive.GameEvent;

namespace WeWillSurvive
{
    [CustomEditor(typeof(MainEventData))]
    public class MainEventDataEditor : Editor
    {
        private const float ICON_FIXED_WIDTH = 64;
        private const float ICON_FIXED_HEIGHT = 64;

        private Dictionary<EChoiceIcon, Texture2D> _iconTextures = new();

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

            var eventIdProp = serializedObject.FindProperty("_eventId");
            var titleProp = serializedObject.FindProperty("_title");
            var descriptionsProp = serializedObject.FindProperty("_descriptions");
            var triggerConditionsProp = serializedObject.FindProperty("_conditions");
            var choiceSchemaProp = serializedObject.FindProperty("_choiceSchema");

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
            var currentChoiceSchema = choiceSchemaProp.intValue;
            EditorGUILayout.PropertyField(choiceSchemaProp, new GUIContent("이벤트 타입"));

            if (currentChoiceSchema != choiceSchemaProp.intValue || data.Choices == null)
            {
                serializedObject.ApplyModifiedProperties(); // 변경된 enum 값을 실제 객체에 먼저 반영

                // 변경된 최신 값을 가져와서 초기화 함수를 호출
                Undo.RecordObject(data, "Initialize Choices for EventType");
                InitializeChoicesForEventType((EMainEventChoiceSchema)choiceSchemaProp.intValue, data);
                EditorUtility.SetDirty(data);

                serializedObject.Update();
            }

            // 이벤트 타입 별 UI 변경
            EditorGUILayout.Space(20);
            SerializedProperty choicesProp = serializedObject.FindProperty("_choices");
            var choiceSchema = (EMainEventChoiceSchema)choiceSchemaProp.intValue;

            switch (choiceSchema)
            {
                case EMainEventChoiceSchema.YesOrNo:
                case EMainEventChoiceSchema.SendSomeone:
                case EMainEventChoiceSchema.Invasion:
                case EMainEventChoiceSchema.Exploration:
                case EMainEventChoiceSchema.Noting:
                    DrawStaticChoiceUI(choicesProp);
                    break;
                case EMainEventChoiceSchema.UseItems:
                    DrawEditableChoiceUI(choicesProp, GetItemNames());
                    break;
                case EMainEventChoiceSchema.ChooseSomeone:
                    DrawEditableChoiceUI(choicesProp, GetCharacterNames());
                    break;
                default:
                    EditorGUILayout.PropertyField(choicesProp);
                    break;
            }

            if (choicesProp.arraySize > 0)
            {
                // 선택한 선택지 결과 출력
                SerializedProperty selectedChoiceProp = choicesProp.GetArrayElementAtIndex(selectedChoiceIndex);
                SerializedProperty resultsProp = selectedChoiceProp.FindPropertyRelative("_results");
                EditorGUILayout.Space(20);
                DrawResultTabsUI(resultsProp);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStaticChoiceUI(SerializedProperty choicesProp)
        {
            EditorGUILayout.LabelField("선택지 목록", EditorStyles.boldLabel, GUILayout.Width(80));
            DrawChoiceSelector(choicesProp);
            EditorGUILayout.Space(10);

            if (choicesProp.arraySize > 0)
            {
                SerializedProperty selectedChoiceProp = choicesProp.GetArrayElementAtIndex(selectedChoiceIndex);
                SerializedProperty choiceIconProp = selectedChoiceProp.FindPropertyRelative("_choiceIcon");
                SerializedProperty amountProp = selectedChoiceProp.FindPropertyRelative("_amount");

                // 이벤트 타입 선택
                EditorGUILayout.PropertyField(choiceIconProp, new GUIContent("선택지 타입"));
                EditorGUILayout.PropertyField(amountProp, new GUIContent("필요 갯수"));
            }
        }

        private void DrawEditableChoiceUI(SerializedProperty choicesProp, (string[] displayOptions, List<EChoiceIcon> enumValues) options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("선택지 목록", EditorStyles.boldLabel, GUILayout.Width(80));
            DrawAddRemoveButtonUI(choicesProp, ref selectedChoiceIndex, InitializeEventChoiceProperty);
            EditorGUILayout.EndHorizontal();

            DrawChoiceSelector(choicesProp);
            EditorGUILayout.Space(10);

            if (choicesProp.arraySize > 0)
            {
                SerializedProperty selectedChoiceProp = choicesProp.GetArrayElementAtIndex(selectedChoiceIndex);
                SerializedProperty choiceIconProp = selectedChoiceProp.FindPropertyRelative("_choiceIcon");
                SerializedProperty amountProp = selectedChoiceProp.FindPropertyRelative("_amount");

                DrawFilteredEnumPopup(new GUIContent("선택지 타입"), choiceIconProp, options.displayOptions, options.enumValues);
                EditorGUILayout.PropertyField(amountProp, new GUIContent("필요 갯수"));
            }
        }

        private void DrawChoiceSelector(SerializedProperty choicesProp)
        {
            if (choicesProp == null)
                return;

            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = ICON_FIXED_WIDTH,
                fixedHeight = ICON_FIXED_HEIGHT,
                margin = new RectOffset(10, 10, 5, 5),
                padding = new RectOffset(2, 2, 2, 2),
                wordWrap = true
            };

            if (choicesProp.arraySize == 0)
                EditorGUILayout.HelpBox("선택지를 추가해주세요.", MessageType.Info);

            // 안전 범위 보정
            if (selectedChoiceIndex < 0 || selectedChoiceIndex >= choicesProp.arraySize)
                selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, choicesProp.arraySize - 1);

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < choicesProp.arraySize; i++)
            {
                SerializedProperty choiceProp = choicesProp.GetArrayElementAtIndex(i);
                SerializedProperty choiceIconProp = choiceProp.FindPropertyRelative("_choiceIcon");

                var choiceIconValue = (EChoiceIcon)choiceIconProp.intValue;

                Rect buttonRect = GUILayoutUtility.GetRect(ICON_FIXED_WIDTH, ICON_FIXED_HEIGHT, style);

                if (selectedChoiceIndex == i)
                {
                    EditorGUI.DrawRect(new Rect(buttonRect.x - 2, buttonRect.y - 2, ICON_FIXED_WIDTH + 4, ICON_FIXED_HEIGHT + 4), Color.cyan);
                }

                Texture2D iconTexture = GetIconTexture(choiceIconValue);
                GUIContent content = iconTexture != null ? new GUIContent(iconTexture) : new GUIContent(choiceIconValue.ToString());
                if (GUI.Button(buttonRect, content, style))
                {
                    selectedChoiceIndex = i;
                    GUI.FocusControl(null);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFilteredEnumPopup(GUIContent label, SerializedProperty property, string[] displayOptions, List<EChoiceIcon> enumValues)
        {
            // 현재 저장된 값을 기준으로 목록에서의 인덱스를 찾습니다.
            var currentValue = (EChoiceIcon)property.intValue;
            int currentIndex = enumValues.IndexOf(currentValue);
            if (currentIndex == -1)
            {
                currentIndex = 0;

                if (enumValues.Count > 0)
                {
                    property.intValue = (int)enumValues[0];
                }
            }

            int newIndex = EditorGUILayout.Popup(label, currentIndex, displayOptions);
            if (newIndex != currentIndex)
            {
                property.intValue = (int)enumValues[newIndex];
            }
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


            // 안전 범위 보정
            if (selectedResultIndex < 0 || selectedResultIndex >= resultsProp.arraySize)
                selectedResultIndex = Mathf.Clamp(selectedResultIndex, 0, resultsProp.arraySize - 1);


            // 탭 버튼 영역
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < resultsProp.arraySize; i++)
            {
                string tabName = $"결과 {i + 1}";
                bool isCurrentlySelected = (selectedResultIndex == i);

                bool isNowSelected = GUILayout.Toggle(isCurrentlySelected, tabName, "Button", GUILayout.MaxWidth(100));

                if (isNowSelected && !isCurrentlySelected)
                {
                    GUI.FocusControl(null);
                    selectedResultIndex = i;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);

            // 선택된 결과 출력
            if (resultsProp.arraySize > 0)
            {
                SerializedProperty selectedResult = resultsProp.GetArrayElementAtIndex(selectedResultIndex);
                if (selectedResult == null) return;

                SerializedProperty conditionsProp = selectedResult.FindPropertyRelative("_conditions");
                SerializedProperty textProp = selectedResult.FindPropertyRelative("_resultText");
                SerializedProperty actionsProp = selectedResult.FindPropertyRelative("_actions");
                SerializedProperty probProp = selectedResult.FindPropertyRelative("_probability");

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
                EditorGUILayout.PropertyField(actionsProp, new GUIContent("결과 액션"), true);
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
                SerializedProperty newElement = prop.GetArrayElementAtIndex(insertIndex);
                onElementInserted?.Invoke(newElement);

                selectedIndex = insertIndex;
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

        private void InitializeChoicesForEventType(EMainEventChoiceSchema choiceSchema, MainEventData data)
        {
            selectedChoiceIndex = 0;
            selectedResultIndex = 0;

            switch (choiceSchema)
            {
                case EMainEventChoiceSchema.YesOrNo:
                    {
                        data.Choices = new List<EventChoice>
                        {
                            new EventChoice(EChoiceIcon.Yes, 0),
                            new EventChoice(EChoiceIcon.No, 0),
                        };
                    }
                    break;
                case EMainEventChoiceSchema.SendSomeone:
                    {
                        data.Choices = new List<EventChoice>
                        {
                            new EventChoice(EChoiceIcon.Lead),
                            new EventChoice(EChoiceIcon.Cook),
                            new EventChoice(EChoiceIcon.Bell),
                            new EventChoice(EChoiceIcon.DrK),
                            new EventChoice(EChoiceIcon.Noting, 0),
                        };
                    }
                    break;
                case EMainEventChoiceSchema.Invasion:
                    {
                        data.Choices = new List<EventChoice>
                        {
                            new EventChoice(EChoiceIcon.Gun),
                            new EventChoice(EChoiceIcon.Pipe),
                            new EventChoice(EChoiceIcon.Ax),
                            new EventChoice(EChoiceIcon.Noting, 0),
                        };
                    }
                    break;
                case EMainEventChoiceSchema.Exploration:
                    {
                        data.Choices = new List<EventChoice>
                        {
                            new EventChoice(EChoiceIcon.Flashlight),
                            new EventChoice(EChoiceIcon.Hand, 0),
                        };
                    }
                    break;
                case EMainEventChoiceSchema.Noting:
                    {
                        data.Choices = new List<EventChoice>
                        {
                            new EventChoice(EChoiceIcon.Noting, 0),
                        };
                    }
                    break;
                default:
                    data.Choices = new List<EventChoice>();
                    break;
            }
        }

        private void InitializeEventChoiceProperty(SerializedProperty choiceProp)
        {
            if (choiceProp == null) return;

            SerializedProperty choiceIconProp = choiceProp.FindPropertyRelative("_choiceIcon");
            SerializedProperty amountProp = choiceProp.FindPropertyRelative("_amount");
            SerializedProperty resultsProp = choiceProp.FindPropertyRelative("_results");

            choiceIconProp.intValue = 0;
            amountProp.intValue = 1;

            resultsProp.ClearArray();
            resultsProp.arraySize = 1;

            SerializedProperty result = resultsProp.GetArrayElementAtIndex(0);
            InitializeEventResultProperty(result);
        }

        private void InitializeEventResultProperty(SerializedProperty resultProp)
        {
            if (resultProp == null) return;

            SerializedProperty textProp = resultProp.FindPropertyRelative("_resultText");
            SerializedProperty probProp = resultProp.FindPropertyRelative("_probability");
            SerializedProperty conditionsProp = resultProp.FindPropertyRelative("_conditions");
            SerializedProperty actionsProp = resultProp.FindPropertyRelative("_actions");

            textProp.stringValue = string.Empty;
            probProp.floatValue = 1.0f;

            conditionsProp.ClearArray();
            actionsProp.ClearArray();
        }

        private void LoadIconTextures()
        {
            _iconTextures.Clear();

            foreach (EChoiceIcon type in Enum.GetValues(typeof(EChoiceIcon)))
            {
                string path = $"Assets/Editor/Icons/{type}.png"; // enum 이름 그대로 사용
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                if (texture != null)
                    _iconTextures[type] = texture;
            }
        }

        private Texture2D GetIconTexture(EChoiceIcon choiceIcon)
        {
            if (_iconTextures.TryGetValue(choiceIcon, out var texture))
                return texture;

            return null;
        }

        private (string[] displayOptions, List<EChoiceIcon> enumValues) GetCharacterNames()
        {
            return GetFilteredEnumOptions(v =>
            {
                int val = (int)v;

                bool isInCharacterRange = (val >= 100 && val < 200);
                bool isNotingItem = (val == 500);

                return isInCharacterRange || isNotingItem;
            });
        }

        private (string[] displayOptions, List<EChoiceIcon> enumValues) GetItemNames()
        {
            return GetFilteredEnumOptions(v =>
            {
                int val = (int)v;

                bool isInItemRange = (val >= 200 && val < 300);
                bool isNotingItem = (val == 500);

                return isInItemRange || isNotingItem;
            });
        }

        private (string[] displayOptions, List<EChoiceIcon> enumValues) GetFilteredEnumOptions(System.Func<EChoiceIcon, bool> filter)
        {
            var allValues = System.Enum.GetValues(typeof(EChoiceIcon)).Cast<EChoiceIcon>();
            var filteredValues = allValues.Where(filter).ToList();

            var displayNames = filteredValues.Select(v =>
            {
                var member = typeof(EChoiceIcon).GetMember(v.ToString())[0];
                var inspectorName = member.GetCustomAttribute<InspectorNameAttribute>();
                return inspectorName != null ? inspectorName.displayName : v.ToString();
            }).ToArray();

            return (displayNames, filteredValues);
        }
    }
}
