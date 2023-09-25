using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QuickTurret.TargetingSystem
{
    [CustomEditor(typeof(TagsAsset))]
    public class TagsAssetEditor : Editor
    {
        GUIStyle centreAlignedLabel;
        bool expandTargetTags = false;
        bool expandSubtargetTags = false;
        bool expandOtherTags = false;
        SerializedProperty tagsProp;

        private void OnEnable()
        {
            serializedObject.Update();

            tagsProp = serializedObject.FindProperty("Tags");
            
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            centreAlignedLabel = new GUIStyle(GUI.skin.label);
            centreAlignedLabel.alignment = TextAnchor.MiddleCenter;

            DisplayInspectorHeader();
            DisplayTargetTags();
            DisplaySubtargetTags();
            DisplayOtherTags();

            serializedObject.ApplyModifiedProperties();
        }

        private void AddNewTag(TargetingTag.TagType type)
        {
            int newElementIndex = tagsProp.arraySize;
            tagsProp.arraySize += 1;

            SerializedProperty newElement = tagsProp.GetArrayElementAtIndex(newElementIndex);
            newElement.FindPropertyRelative("Name").stringValue = "New Tag";
            newElement.FindPropertyRelative("Type").intValue = (int)type;
            newElement.FindPropertyRelative("guid").FindPropertyRelative("serializedGuid").stringValue = System.Guid.NewGuid().ToString();
        }

        private void DisplayInspectorHeader()
        {
            EditorGUILayout.LabelField("Tags", centreAlignedLabel);
            EditorGUILayout.Space(3f);
        }

        private void DisplayOtherTags()
        {
            EditorGUILayout.BeginHorizontal();

            expandOtherTags = EditorGUILayout.BeginFoldoutHeaderGroup(expandOtherTags, "Other Tags");
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (GUILayout.Button("Add", GUILayout.Width(40f)))
                AddNewTag(TargetingTag.TagType.Other);
            
            EditorGUILayout.EndHorizontal();

            if (expandOtherTags)
            {
                EditorGUI.indentLevel += 2;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    SerializedProperty tagProp = tagsProp.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = tagProp.FindPropertyRelative("Name");
                    SerializedProperty typeProp = tagProp.FindPropertyRelative("Type");
                    SerializedProperty guidProp = tagProp.FindPropertyRelative("guid");

                    if (typeProp.enumValueIndex != 0)
                        continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(nameProp);
                    if (GUILayout.Button("Remove", GUILayout.Width(60f), GUILayout.ExpandHeight(true)))
                    {
                        RemoveTag(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(typeProp);
                    EditorGUILayout.PropertyField(guidProp);

                    EditorGUILayout.Space(2f);
                }
                EditorGUI.indentLevel -= 2;
            }
        }

        private void DisplaySubtargetTags()
        {
            EditorGUILayout.BeginHorizontal();

            expandSubtargetTags = EditorGUILayout.BeginFoldoutHeaderGroup(expandSubtargetTags, "Subtarget Tags");
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (GUILayout.Button("Add", GUILayout.Width(40f)))
                AddNewTag(TargetingTag.TagType.Subtarget);

            EditorGUILayout.EndHorizontal();

            if (expandSubtargetTags)
            {
                EditorGUI.indentLevel += 2;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    SerializedProperty tagProp = tagsProp.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = tagProp.FindPropertyRelative("Name");
                    SerializedProperty typeProp = tagProp.FindPropertyRelative("Type");
                    SerializedProperty guidProp = tagProp.FindPropertyRelative("guid");

                    if (typeProp.enumValueIndex != 2)
                        continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(nameProp);
                    if (GUILayout.Button("Remove", GUILayout.Width(60f), GUILayout.ExpandHeight(true)))
                    {
                        RemoveTag(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(typeProp);
                    EditorGUILayout.PropertyField(guidProp);
                    EditorGUILayout.Space(2f);
                }
                EditorGUI.indentLevel -= 2;
            }
        }

        private void DisplayTargetTags()
        {
            EditorGUILayout.BeginHorizontal();
            
            expandTargetTags = EditorGUILayout.BeginFoldoutHeaderGroup(expandTargetTags, "Target Tags");
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (GUILayout.Button("Add", GUILayout.Width(40f)))
                AddNewTag(TargetingTag.TagType.Target);
            
            EditorGUILayout.EndHorizontal();

            if (expandTargetTags)
            {
                EditorGUI.indentLevel += 2;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    SerializedProperty tagProp = tagsProp.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = tagProp.FindPropertyRelative("Name");
                    SerializedProperty typeProp = tagProp.FindPropertyRelative("Type");
                    SerializedProperty guidProp = tagProp.FindPropertyRelative("guid");

                    if (typeProp.enumValueIndex != 1)
                        continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(nameProp);
                    if (GUILayout.Button("Remove", GUILayout.Width(60f), GUILayout.ExpandHeight(true)))
                    {
                        RemoveTag(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(typeProp);
                    EditorGUILayout.PropertyField(guidProp);
                    EditorGUILayout.Space(2f);
                }
                EditorGUI.indentLevel -= 2;
            }
        }

        private void RemoveTag(int i)
        {
            tagsProp.DeleteArrayElementAtIndex(i);
        }
    }
}
