using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(General_UI_DropDown_Handler_ScriptV2))]
public class General_UI_DropDown_Handler_ScriptV2_CustomInspector : Editor
{

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        General_UI_DropDown_Handler_ScriptV2 general_UI_DropDown_Handler_ScriptV2 = (General_UI_DropDown_Handler_ScriptV2)target;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showInfoForDebug"));
        if (general_UI_DropDown_Handler_ScriptV2.getShowInfoForDebug())
        {
            if (!general_UI_DropDown_Handler_ScriptV2.getIsCanvasOrUiItem())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isHolder"));
            }
            if (!general_UI_DropDown_Handler_ScriptV2.getIsHolder())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isCanvasOrUiItem"));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropDownImageFollowChildSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropDownDirection"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("engaged"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mainImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropDownBackgroundImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InteractionButton"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lockInteractionButtonVisuals"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("contentRectTransform"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("childDropDowns"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ChildrenObjectHolder"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("globalOffsetDist"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("itemSeperationDist"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("offsetDist"));
        }
        else if (general_UI_DropDown_Handler_ScriptV2.getIsHolder())
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isHolder"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropDownDirection"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mainImage"));
        }
        else if (general_UI_DropDown_Handler_ScriptV2.getIsCanvasOrUiItem())
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isCanvasOrUiItem"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropDownDirection"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropDownImageFollowChildSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("contentRectTransform"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ChildrenObjectHolder"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("childDropDowns"));
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isHolder"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isCanvasOrUiItem"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropDownImageFollowChildSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropDownDirection"));


            EditorGUILayout.PropertyField(serializedObject.FindProperty("mainImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropDownBackgroundImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InteractionButton"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lockInteractionButtonVisuals"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ChildrenObjectHolder"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("childDropDowns"));

        }



        serializedObject.ApplyModifiedProperties();
    }
}
