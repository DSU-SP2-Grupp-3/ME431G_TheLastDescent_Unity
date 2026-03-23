using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveInRangeCommandWrapper))]
public class MoveInRangeWrapperEditor : Editor
{
    private MoveInRangeCommandWrapper wrapper;
    
    private SerializedProperty range;
    private SerializedProperty relativePosition;

    private float rangeValue => range.floatValue;
    private Vector3 positionValue => wrapper.transform.position + relativePosition.vector3Value;

    private Color color;
    
    public void OnEnable() {
        wrapper = target as MoveInRangeCommandWrapper;
        range = serializedObject.FindProperty("range");
        relativePosition = serializedObject.FindProperty("relativePosition");
        
        color = Color.green;
        color.a = 0.2f;
    }

    public void OnSceneGUI()
    {
        Handles.color = color;
        // Handles.DrawSolidDisc(positionValue, Vector3.up, rangeValue);

        range.floatValue = Handles.RadiusHandle(Quaternion.identity, positionValue, rangeValue, false);
        
        if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
    }
}