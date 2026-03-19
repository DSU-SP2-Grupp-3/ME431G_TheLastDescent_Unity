using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(HideIfAttribute))]
public class HideIfAttributeDrawer : PropertyDrawer
{
    private bool Hidden(SerializedProperty property)
    {
        return Hidden(property.serializedObject);
    }

    private bool Hidden(SerializedObject serializedObject)
    {
        HideIfAttribute hideIf = attribute as HideIfAttribute;

        MethodInfo method = serializedObject.targetObject.GetType().GetMethod(
            hideIf.predicateName
        );

        Object instance = serializedObject.targetObject as Object;


        bool hide = false;

        if (method != null && method.ReturnType == typeof(bool))
        {
            hide = (bool)method.Invoke(instance, Array.Empty<object>());
        }

        return hide;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!Hidden(property))
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();
        
        container.Add(new PropertyField(property));
        
        return container;
        
        // only works for IMGUI right now, below code must be fixed so property is redrawn when the object changes
        
        // if (!Hidden(property))
        // {
        //     container.Add(new PropertyField(property));
        // }
        //
        //
        // // container.Bind(property.serializedObject);
        // container.TrackSerializedObjectValue(property.serializedObject, so =>
        // {
        //     Debug.Log("track");
        //     container.Clear();
        //     if (!Hidden(so))
        //     {
        //         container.Add(new PropertyField(so.FindProperty(property.propertyPath)));
        //     }
        //     container.MarkDirtyRepaint();
        // });
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return Hidden(property) ? 0f : base.GetPropertyHeight(property, label);
    }
}