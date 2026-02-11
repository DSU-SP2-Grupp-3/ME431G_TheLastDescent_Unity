using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UEC_SFX))]
public class EditorUEC_SFX : Editor
{
   #region Properties

   //Enum
   SerializedProperty actionTypeSelectProp;
   SerializedProperty findMethodTypeProp;
   SerializedProperty parametersProp;
   
   //String/StringArray
   SerializedProperty objectTagProp;
   SerializedProperty objectTagsProp;
   
   //GameObjects
   SerializedProperty uecObjectProp;
   SerializedProperty uecObjectsProp;
   

   #endregion

   private void OnEnable()
   {
      actionTypeSelectProp = serializedObject.FindProperty("action");
      findMethodTypeProp = serializedObject.FindProperty("findMethod");
      parametersProp = serializedObject.FindProperty("parameters");
      objectTagProp = serializedObject.FindProperty("objectTag");
      objectTagsProp = serializedObject.FindProperty("objectTags");
      uecObjectProp = serializedObject.FindProperty("uecObject");
      uecObjectsProp = serializedObject.FindProperty("uecObjects");
   }

   private void ShowActionType()
   {
      EditorGUILayout.PropertyField(actionTypeSelectProp);
      if (actionTypeSelectProp.enumValueIndex == 0) //Play
      {
         
      }
      else if (actionTypeSelectProp.enumValueIndex == 1) //Stop
      {
         
      }
      else if (actionTypeSelectProp.enumValueIndex == 2) //SetParameter 
      {
         EditorGUILayout.PropertyField(parametersProp);
      }
   }

   public override void OnInspectorGUI()
   {
      //Content
      serializedObject.Update();
      
      EditorGUILayout.LabelField("SFX", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(findMethodTypeProp);
      if (findMethodTypeProp.enumValueIndex == 0)
      {
         EditorGUILayout.LabelField("GameObject(s)", EditorStyles.boldLabel);
         EditorGUILayout.PropertyField(uecObjectsProp);
      }
      else if (findMethodTypeProp.enumValueIndex == 1)
      {
         EditorGUILayout.LabelField("Find Object with Tag(s)", EditorStyles.boldLabel);
      }
      else if (findMethodTypeProp.enumValueIndex == 2)
      {
         EditorGUILayout.LabelField("Find Objects", EditorStyles.boldLabel);
      }
      
      serializedObject.ApplyModifiedProperties();
   }
}
