using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using QuickTurret.DamageSystem;

[CustomPropertyDrawer(typeof(DamageTextSettings.OverrideSetting))]
public class DamageTextSettingsOverridePropertyDrawer : PropertyDrawer
{
    readonly float A = EditorGUIUtility.singleLineHeight;
    readonly float B = EditorGUIUtility.standardVerticalSpacing;

    private bool isExpanded = false;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!isExpanded)
            return 2 * A + B;

        var doesOverrideProperty = property.FindPropertyRelative("DoesOverride");

        int numTrue = 0;
        for (int i = 0; i < doesOverrideProperty.arraySize; i++)
        {
            if (doesOverrideProperty.GetArrayElementAtIndex(i).boolValue)
                numTrue += 1;
        }

        return 10 * A + 9 * B + numTrue * (A + B);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect previousRect = new Rect(position.x, position.y, position.width, A);

        // Faux horizontal separator
        EditorGUI.LabelField(previousRect, "", GUI.skin.horizontalSlider);

        var damageTypeProperty = property.FindPropertyRelative("DamageType");
        string damageType = damageTypeProperty.enumDisplayNames[damageTypeProperty.enumValueIndex];
        previousRect.y += A + B;

        isExpanded = EditorGUI.BeginFoldoutHeaderGroup(previousRect, isExpanded, new GUIContent($"Damage Type: {damageType}"));
        EditorGUI.EndFoldoutHeaderGroup();

        if (!isExpanded)
            return;

        previousRect.y += A + B;
        EditorGUI.PropertyField(previousRect, property.FindPropertyRelative("Priority"));

        var doesOverrideProperty = property.FindPropertyRelative("DoesOverride");

        var doesOverrideFontProperty = doesOverrideProperty.GetArrayElementAtIndex(0);
        previousRect.y += A + B;
        doesOverrideFontProperty.boolValue = EditorGUI.ToggleLeft(previousRect, "Override Font Asset", doesOverrideFontProperty.boolValue);

        if (doesOverrideFontProperty.boolValue)
        {
            previousRect.y += A + B;
            EditorGUI.PropertyField(previousRect, property.FindPropertyRelative("FontAsset"));
        }

        var doesOverrideFontSizeProperty = doesOverrideProperty.GetArrayElementAtIndex(1);
        previousRect.y += A + B;
        doesOverrideFontSizeProperty.boolValue = EditorGUI.ToggleLeft(previousRect, "Override Font Size", doesOverrideFontSizeProperty.boolValue);

        if (doesOverrideFontSizeProperty.boolValue)
        {
            previousRect.y += A + B;
            EditorGUI.PropertyField(previousRect, property.FindPropertyRelative("FontSize"));
        }

        var doesOverrideFontColorProperty = doesOverrideProperty.GetArrayElementAtIndex(2);
        previousRect.y += A + B;
        doesOverrideFontColorProperty.boolValue = EditorGUI.ToggleLeft(previousRect, "Override Font Color", doesOverrideFontColorProperty.boolValue);

        if (doesOverrideFontColorProperty.boolValue)
        {
            previousRect.y += A + B;
            EditorGUI.PropertyField(previousRect, property.FindPropertyRelative("FontColor"));
        }

        var doesOverrideFontOutlineColorProperty = doesOverrideProperty.GetArrayElementAtIndex(3);
        previousRect.y += A + B;
        doesOverrideFontOutlineColorProperty.boolValue = EditorGUI.ToggleLeft(previousRect, "Override Font Outline Color", doesOverrideFontOutlineColorProperty.boolValue);

        if (doesOverrideFontOutlineColorProperty.boolValue)
        {
            previousRect.y += A + B;
            EditorGUI.PropertyField(previousRect, property.FindPropertyRelative("FontOutlineColor"));
        }

        var doesOverrideTweenClimbAmountProperty = doesOverrideProperty.GetArrayElementAtIndex(4);
        previousRect.y += A + B;
        doesOverrideTweenClimbAmountProperty.boolValue = EditorGUI.ToggleLeft(previousRect, "Override Text Tween Climb Amount", doesOverrideTweenClimbAmountProperty.boolValue);

        if (doesOverrideTweenClimbAmountProperty.boolValue)
        {
            previousRect.y += A + B;
            EditorGUI.PropertyField(previousRect, property.FindPropertyRelative("TweenClimbAmount"));
        }

        var doesOverrideTweenLifetimeProperty = doesOverrideProperty.GetArrayElementAtIndex(5);
        previousRect.y += A + B;
        doesOverrideTweenLifetimeProperty.boolValue = EditorGUI.ToggleLeft(previousRect, "Override Text Tween Lifetime", doesOverrideTweenLifetimeProperty.boolValue);

        if (doesOverrideTweenLifetimeProperty.boolValue)
        {
            previousRect.y += A + B;
            EditorGUI.PropertyField(previousRect, property.FindPropertyRelative("TweenLifetime"));
        }

        var doesOverrideTweenRandomnessProperty = doesOverrideProperty.GetArrayElementAtIndex(6);
        previousRect.y += A + B;
        doesOverrideTweenRandomnessProperty.boolValue = EditorGUI.ToggleLeft(previousRect, "Override Text Tween Randomness", doesOverrideTweenRandomnessProperty.boolValue);

        if (doesOverrideTweenRandomnessProperty.boolValue)
        {
            previousRect.y += A + B;
            EditorGUI.PropertyField(previousRect, property.FindPropertyRelative("TweenRandomness"));
        }
    }
}
