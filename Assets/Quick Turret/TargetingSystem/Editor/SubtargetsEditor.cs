using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using QuickTurret.TargetingSystem;

[CustomEditor(typeof(Subtarget))]
public class SubtargetsEditor : Editor
{
    bool expandTags = true;

    SerializedProperty targetableProperty => serializedObject.FindProperty("targetable");
    Targetable targetable
    {
        get => targetableProperty.objectReferenceValue as Targetable;
        set => targetableProperty.objectReferenceValue = value;
    } 
    
    SerializedProperty tagsProperty => serializedObject.FindProperty("tags");

    SerializedProperty positionProperty => serializedObject.FindProperty("position");

    TagsAsset tagsAsset
    {
        get
        {
            if (targetable == null)
                return null;
            else
                return targetable.TagsAsset;
        }
    }

    #region Inspector Behaviour

    public override void OnInspectorGUI()
    {
        DisplayTargetableReference();
        DisplaySubtargetTags();
        DisplayPosition();
    }

    private void DisplayPosition()
    {
        if (targetable == null)
            return;

        // Faux horizontal separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        serializedObject.Update();

        EditorGUILayout.PropertyField(positionProperty);

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplaySubtargetTag(TargetingTag tag)
    {
        serializedObject.Update();

        bool wasTaggedBefore = DoesSubtargetHaveTag(tag);
        bool hasTag = wasTaggedBefore;

        hasTag = EditorGUILayout.ToggleLeft($"{tag.Name} ({tag.Type})", hasTag);

        if (wasTaggedBefore && !hasTag)
        {
            // Remove the tag from the tags property.
            for (int j = 0; j < tagsProperty.arraySize; j++)
            {
                var loopTag = tagsProperty.GetArrayElementAtIndex(j);
                string loopTagGUID = loopTag.FindPropertyRelative("guid").FindPropertyRelative("serializedGuid").stringValue;
                if (loopTagGUID == tag.GUID.ToString())
                    tagsProperty.DeleteArrayElementAtIndex(j);
            }
        }
        else if (!wasTaggedBefore && hasTag)
        {
            // Add the tag to the tags property.
            int newTagIndex = tagsProperty.arraySize;
            tagsProperty.arraySize += 1;
            var newTagProp = tagsProperty.GetArrayElementAtIndex(newTagIndex);
            newTagProp.FindPropertyRelative("Name").stringValue = tag.Name;
            newTagProp.FindPropertyRelative("Type").enumValueIndex = (int)tag.Type;
            newTagProp.FindPropertyRelative("guid").FindPropertyRelative("serializedGuid").stringValue = tag.GUID.ToString();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplaySubtargetTags()
    {
        if (tagsAsset == null)
            return;

        // Faux horizontal separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        expandTags = EditorGUILayout.BeginFoldoutHeaderGroup(expandTags, "Tags");
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (!expandTags)
            return;

        serializedObject.Update();

        for (int i = 0; i < tagsAsset.Tags.Count; i++)
        {
            TargetingTag tag = tagsAsset.Tags[i];
            if (tag.Type != TargetingTag.TagType.Subtarget && tag.Type != TargetingTag.TagType.Other)
                continue;

            DisplaySubtargetTag(tag);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayTargetableReference()
    {
        serializedObject.Update();

        Targetable oldTargetable = targetable;

        if (EditorUtility.IsPersistent(target))
            targetable = EditorGUILayout.ObjectField("Targetable", targetable, typeof(Targetable), false) as Targetable;
        else
            targetable = EditorGUILayout.ObjectField("Targetable", targetable, typeof(Targetable), true) as Targetable;

        if (oldTargetable != targetable && oldTargetable != null)
            oldTargetable.DeregisterSubtarget(target as Subtarget);

        serializedObject.ApplyModifiedProperties();
    }

    private bool DoesSubtargetHaveTag(TargetingTag tag)
    {
        if (tagsAsset == null)
            return false;

        for (int i = 0; i < tagsProperty.arraySize; i++)
        {
            SerializedProperty loopTag = tagsProperty.GetArrayElementAtIndex(i);
            string loopTagGUID = loopTag.FindPropertyRelative("guid").FindPropertyRelative("serializedGuid").stringValue;

            if (tag.GUID.ToString() == loopTagGUID)
                return true;
        }

        return false;
    }

    #endregion

    #region Scene View Behaviour

    private void OnSceneGUI()
    {
        DrawSubtargetHandle();
    }

    public void DrawSubtargetHandle()
    {
        if (targetable == null)
            return;

        Subtarget subtarget = target as Subtarget;
        if (subtarget.Targetable.gameObject == (target as Subtarget).gameObject)
            return;

        Vector3 pos = subtarget.WorldPosition;

        Handles.Label(pos + 0.5f * Vector3.up, $"{subtarget.gameObject.name} subtarget");

        Handles.color = Color.white;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
        Handles.SphereHandleCap(0, pos, Quaternion.identity, 0.25f, EventType.Repaint);

        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        Handles.color = new Color(0.75f, 0f, 0f, 1f);
        Handles.DrawLine(pos - 0.3f * Vector3.right, pos + 0.3f * Vector3.right);
        Handles.DrawLine(pos - 0.3f * Vector3.up, pos + 0.3f * Vector3.up);
        Handles.DrawLine(pos - 0.3f * Vector3.forward, pos + 0.3f * Vector3.forward);
    }

    #endregion
}
