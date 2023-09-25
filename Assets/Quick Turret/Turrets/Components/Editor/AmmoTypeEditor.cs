using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using QuickTurret.TurretComponents;
using QuickTurret.DamageSystem;

[CustomEditor(typeof(AmmoType))]
public class AmmoTypeEditor : Editor
{
    SerializedProperty SerializedChancesProp { get { return serializedObject.FindProperty("serializedChances"); } }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        string[] names = System.Enum.GetNames(typeof(DamageEffect.DamageType));
        for (int i = 1; i < names.Length; i++)
        {
            string name = names[i];
             
            // We ask for element (i - 1) here because our chances list has no entry
            // for 'Normal' type damage, and instead starts at the second type of 
            // damage defined in DamageEffect.DamageType.
            var chanceProp = SerializedChancesProp.GetArrayElementAtIndex(i - 1);
             
            EditorGUILayout.PropertyField(chanceProp, new GUIContent($"{name} chance"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
