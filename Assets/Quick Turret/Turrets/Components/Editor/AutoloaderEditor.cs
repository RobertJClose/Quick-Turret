using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QuickTurret.TurretComponents
{
    [CustomEditor(typeof(Autoloader))]
    public class AutoloaderEditor : Editor
    {
        GUIStyle centreAlignedLabel;
        SerializedProperty ammoProp;
        SerializedProperty sizeProp;

        int pageNumber;
        int roundsPerPage = 5;

        int NumberOfRequiredPages { get { return (sizeProp.intValue - 1) / roundsPerPage + 1; } }

        SerializedProperty ReloadTimeProperty => serializedObject.FindProperty("ReloadTime");

        private void OnEnable()
        {
            serializedObject.Update();

            ammoProp = serializedObject.FindProperty("ammoBelt");
            sizeProp = serializedObject.FindProperty("beltSize");
        }

        private void Awake()
        {
            pageNumber = 1;
        }

        public override void OnInspectorGUI()
        {
            centreAlignedLabel = new GUIStyle(GUI.skin.label);
            centreAlignedLabel.alignment = TextAnchor.MiddleCenter;

            serializedObject.Update();

            EditorGUILayout.PropertyField(ReloadTimeProperty);

            ShowBeltHeader();
            ShowBeltSizeAdjuster();
            ShowPageNumberButtons();
            ShowRounds();

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowBeltHeader()
        {
            // Faux horizontal separator.
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Ammo Belt", centreAlignedLabel);
        }

        private void ShowBeltSizeAdjuster()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("-"))
                sizeProp.intValue = Mathf.Max(0, sizeProp.intValue - 1);

            EditorGUILayout.LabelField($"Belt Size: {sizeProp.intValue}", centreAlignedLabel);

            if (GUILayout.Button("+"))
                sizeProp.intValue += 1;

            EditorGUILayout.EndHorizontal();

            if (ammoProp.arraySize > sizeProp.intValue)
            {
                for (int i = ammoProp.arraySize - 1; i >= sizeProp.intValue; i--)
                {
                    ammoProp.DeleteArrayElementAtIndex(i);
                }
            }
            else if (ammoProp.arraySize < sizeProp.intValue)
                ammoProp.arraySize = sizeProp.intValue;

            if (pageNumber > NumberOfRequiredPages)
                pageNumber = NumberOfRequiredPages;
        }

        private void ShowPageNumberButtons()
        {
            if (NumberOfRequiredPages == 1)
                return;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<"))
                pageNumber = Mathf.Max(pageNumber - 1, 1);

            EditorGUILayout.LabelField($"Page {pageNumber}", centreAlignedLabel);

            if (GUILayout.Button(">"))
                pageNumber = Mathf.Min(pageNumber + 1, NumberOfRequiredPages);

            EditorGUILayout.EndHorizontal();
        }

        private void ShowRounds()
        {
            for (int i = (pageNumber - 1) * roundsPerPage;
                i < pageNumber * roundsPerPage && i < ammoProp.arraySize;
                i++)
            {
                SerializedProperty property = ammoProp.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(property, new GUIContent($"Round {i + 1}"));
            }
        }
    }
}
