using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using QuickTurret.TargetingSystem;

[CustomEditor(typeof(Targetable))]
public class TargetableEditor : Editor
{
    private bool expandSubtargets;
    private bool expandTags;

    private SerializedProperty subtargetsProperty => serializedObject.FindProperty("subtargets");

    private SerializedProperty tagsProp;
    private SerializedProperty tagsAssetProp;
    
    private TagsAsset TagsAsset { get { return tagsAssetProp.objectReferenceValue as TagsAsset; } }

    private void OnEnable()
    {
        serializedObject.Update();

        expandSubtargets = true;
        expandTags = true;
        tagsProp = serializedObject.FindProperty("tags");
        tagsAssetProp = serializedObject.FindProperty("tagsAsset");

        serializedObject.ApplyModifiedProperties();
    }

    #region Inspector Behaviour

    public override void OnInspectorGUI()
    {
        DisplayTagsAsset();
        DisplayTargetTags();
        DisplaySubtargets();
    }

    private void DisplaySubtargets()
    {
        // Faux horizontal separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        expandSubtargets = EditorGUILayout.BeginFoldoutHeaderGroup(expandSubtargets, "Subtargets");
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (!expandSubtargets)
            return;

        for (int i = 0; i < subtargetsProperty.arraySize; i++)
        {
            Subtarget subtarget = subtargetsProperty.GetArrayElementAtIndex(i).objectReferenceValue as Subtarget;

            if (EditorGUILayout.LinkButton($"{subtarget.gameObject.name}"))
                Selection.activeGameObject = subtarget.gameObject;
        }
    }

    private void DisplayTagsAsset()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(tagsAssetProp);
        EditorGUILayout.Space(2f);

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayTargetTag(TargetingTag tag)
    {
        serializedObject.Update();

        bool wasTaggedBefore = DoesTargetHaveTag(tag);
        bool hasTag = wasTaggedBefore;

        hasTag = EditorGUILayout.ToggleLeft($"{tag.Name} ({tag.Type})", hasTag);

        if (wasTaggedBefore && !hasTag)
        {
            // Remove the tag from the tags property.
            for (int j = 0; j < tagsProp.arraySize; j++)
            {
                var loopTag = tagsProp.GetArrayElementAtIndex(j);
                string loopTagGUID = loopTag.FindPropertyRelative("guid").FindPropertyRelative("serializedGuid").stringValue;
                if (loopTagGUID == tag.GUID.ToString())
                    tagsProp.DeleteArrayElementAtIndex(j);
            }
        }
        else if (!wasTaggedBefore && hasTag)
        {
            // Add the tag to the tags property.
            int newTagIndex = tagsProp.arraySize;
            tagsProp.arraySize += 1;
            var newTagProp = tagsProp.GetArrayElementAtIndex(newTagIndex);
            newTagProp.FindPropertyRelative("Name").stringValue = tag.Name;
            newTagProp.FindPropertyRelative("Type").enumValueIndex = (int)tag.Type;
            newTagProp.FindPropertyRelative("guid").FindPropertyRelative("serializedGuid").stringValue = tag.GUID.ToString();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayTargetTags()
    {
        if (TagsAsset == null)
            return;

        // Faux horizontal separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        expandTags = EditorGUILayout.BeginFoldoutHeaderGroup(expandTags, "Tags");
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (!expandTags) 
            return;

        serializedObject.Update();

        for (int i = 0; i < TagsAsset.Tags.Count; i++)
        {
            TargetingTag tag = TagsAsset.Tags[i];
            if (tag.Type != TargetingTag.TagType.Target && tag.Type != TargetingTag.TagType.Other)
                continue;

            DisplayTargetTag(tag);
        }

        serializedObject.ApplyModifiedProperties();
    }
    
    private bool DoesTargetHaveTag(TargetingTag tag)
    {
        if (TagsAsset == null)
            return false;

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty loopTag = tagsProp.GetArrayElementAtIndex(i);
            string loopTagGUID = loopTag.FindPropertyRelative("guid").FindPropertyRelative("serializedGuid").stringValue;

            if (tag.GUID.ToString() == loopTagGUID)
                return true;
        }

        return false;
    }

    #endregion

    #region Scene Behaviour

    private void OnSceneGUI()
    {
        DrawSubtargetsHandles();
    }

    private void DrawSubtargetsHandles()
    {
        for (int i = 0; i < subtargetsProperty.arraySize; i++)
        {
            Subtarget subtarget = subtargetsProperty.GetArrayElementAtIndex(i).objectReferenceValue as Subtarget;

            Vector3 pos = subtarget.WorldPosition;

            Handles.Label(pos + 0.5f * Vector3.up, $"{subtarget.gameObject.name} subtarget");

            Handles.color = subtarget.gameObject == (target as Targetable).gameObject ? Color.green : Color.white;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            Handles.SphereHandleCap(0, pos, Quaternion.identity, 0.25f, EventType.Repaint);

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            Handles.color = new Color(0.75f, 0f, 0f, 1f);
            Handles.DrawLine(pos - 0.3f * Vector3.right, pos + 0.3f * Vector3.right);
            Handles.DrawLine(pos - 0.3f * Vector3.up, pos + 0.3f * Vector3.up);
            Handles.DrawLine(pos - 0.3f * Vector3.forward, pos + 0.3f * Vector3.forward);
        }
    }

    #endregion
}
