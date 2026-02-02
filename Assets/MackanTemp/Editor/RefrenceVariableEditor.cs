using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatRefrence))]
public class RefrenceVariableEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty useConstant = property.FindPropertyRelative("useConstant");
        SerializedProperty constantValue = property.FindPropertyRelative("constantValue");
        SerializedProperty fvar = property.FindPropertyRelative("fvar");

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        Rect buttonRect = new Rect(position.x - 22, position.y, 20, position.height);
        Rect fieldRect = new Rect(position.x, position.y, position.width, position.height);

        int result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, new[] { "Const", "FloatVar" });
        useConstant.boolValue = (result == 0);

        if (useConstant.boolValue)
            EditorGUI.PropertyField(fieldRect, constantValue, GUIContent.none);
        else
            EditorGUI.PropertyField(fieldRect, fvar, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
