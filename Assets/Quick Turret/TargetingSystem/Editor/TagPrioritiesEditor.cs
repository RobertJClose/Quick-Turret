using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using QuickTurret.TargetingSystem;

[CustomEditor(typeof(TagPriorities))]
public class TagPrioritiesEditor : Editor
{
    SerializedProperty noTagPriorityValue;
    SerializedProperty priorityListProp;
    SerializedProperty tagsAssetProp;
    
    private TagsAsset TagsAsset { get { return tagsAssetProp.objectReferenceValue as TagsAsset; } }

    private void OnEnable()
    {
        serializedObject.Update();
        noTagPriorityValue = serializedObject.FindProperty("serializedNoTagPriorityValue");
        priorityListProp = serializedObject.FindProperty("priorityList");
        tagsAssetProp = serializedObject.FindProperty("tagsAsset");
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        DisplayTagsAsset();
        DisplayHelpBox();
        DisplayNoTagPriority();

        if (TagsAsset != null)
        {
            foreach (var tag in TagsAsset.Tags)
            {
                DisplayTagPriority(tag);
            }
        }
    }

    private void DisplayHelpBox()
    {
        EditorGUILayout.Space(2f);

        if (TagsAsset == null)        
            EditorGUILayout.HelpBox("Select a TagsAsset to get started.", MessageType.None);        
        else        
            EditorGUILayout.HelpBox("Lower values means that Targetables/Subtargets with those tags are considered to be a higher priority target.", MessageType.None);        

        EditorGUILayout.Space(2f);
    }

    private void DisplayNoTagPriority()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();

        noTagPriorityValue.intValue = EditorGUILayout.IntField("No tag", noTagPriorityValue.intValue);

        if (GUILayout.Button("Min", GUILayout.Width(40f)))
            noTagPriorityValue.intValue = int.MinValue;
        if (GUILayout.Button("Max", GUILayout.Width(40f)))
            noTagPriorityValue.intValue = int.MaxValue;

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayTagPriority(TargetingTag tag)
    {
        EditorGUILayout.BeginHorizontal();

        var priority = FindPriority(tag);

        priority.PriorityValue = EditorGUILayout.IntField($"Tag: {tag.Name} ({tag.Type})", priority.PriorityValue);
        if (GUILayout.Button("Min", GUILayout.Width(40f)))
            priority.PriorityValue = int.MinValue;
        if (GUILayout.Button("Max", GUILayout.Width(40f)))
            priority.PriorityValue = int.MaxValue;

        EditorGUILayout.EndHorizontal();
    }

    private void DisplayTagsAsset()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(tagsAssetProp);
        EditorGUILayout.Space(2f);

        serializedObject.ApplyModifiedProperties();
    }

    private TagPriorities.Priority FindPriority(TargetingTag tag)
    {
        for (int i = 0; i < priorityListProp.arraySize; i++)
        {
            var loopPriorityProp = priorityListProp.GetArrayElementAtIndex(i);
            TagPriorities.Priority loopPriority = loopPriorityProp.managedReferenceValue as TagPriorities.Priority;
            
            if (tag == loopPriority.Tag)
            {
                return loopPriority;
            }
        }

        Debug.LogWarning($"Could not find a priority setting in {serializedObject.targetObject.name}!\n Tags Asset: {TagsAsset.name}. Tag {tag.Name}");
        return null;
    }
}
