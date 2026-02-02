using System;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InspectTest))]
public class EditorInspectTest : Editor
{
    
    GUIStyle style;
    GUILayoutOption[] options;
    
    #region MyRegion

    SerializedProperty forEnum;
    SerializedProperty forTest;

    SerializedProperty DropDown1;
    SerializedProperty DropDown2;
    SerializedProperty DropDown3;

    private bool testGroup = false;

    #endregion

    private void OnEnable()
    {
        forEnum = serializedObject.FindProperty("forEnum");
        forTest = serializedObject.FindProperty("forTest");
        
        DropDown1 = serializedObject.FindProperty("dropDown1");
        DropDown2 = serializedObject.FindProperty("dropDown2");
        DropDown3 = serializedObject.FindProperty("dropDown3");
    }

    public override void OnInspectorGUI()
    {
        //Content
        
        style.fontStyle = FontStyle.Bold;
        
        InspectTest inspectTest = (InspectTest)target;
        
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Test");
        
        testGroup = EditorGUILayout.BeginFoldoutHeaderGroup(testGroup, "Test Group");
        if (testGroup)
        {
            EditorGUILayout.PropertyField(forEnum);
            if (inspectTest.forEnum)
            {
                EditorGUILayout.PropertyField(DropDown1);
                if (DropDown1.enumValueIndex == 0)
                {
                    EditorGUILayout.LabelField("Drop2");
                    EditorGUILayout.PropertyField(DropDown2);
                }

                else if (DropDown1.enumValueIndex == 1)
                {
                    EditorGUILayout.LabelField("Drop3");
                    EditorGUILayout.PropertyField(DropDown3);
                    if (DropDown3.enumValueIndex == 1)
                    {
                        EditorGUILayout.PropertyField(forTest);
                    }
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        EditorGUILayout.LabelField("Cake");
        
        serializedObject.ApplyModifiedProperties();

        
    }
}
