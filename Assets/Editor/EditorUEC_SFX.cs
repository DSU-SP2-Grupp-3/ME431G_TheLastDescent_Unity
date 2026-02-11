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
   
   //String/StringArray
   SerializedProperty objectTagsProp;
   
   //GameObjects
   SerializedProperty uecObjectsProp;
   
   //Structs
   SerializedProperty commandProp;
   
   //Booleans
   

   #endregion

   private void OnEnable()
   {
      actionTypeSelectProp = serializedObject.FindProperty("action");
      findMethodTypeProp = serializedObject.FindProperty("findMethod");
      objectTagsProp = serializedObject.FindProperty("objectTags");
      uecObjectsProp = serializedObject.FindProperty("uecObjects");
      commandProp = serializedObject.FindProperty("commands");
   }
   private void ShowCommands()
   {
      EditorGUILayout.LabelField("Commands", EditorStyles.boldLabel);

      for (int i = 0; i < commandProp.arraySize; i++)
      {
         var commandsProp = commandProp.GetArrayElementAtIndex(i);
         var actionTypeProp = commandsProp.FindPropertyRelative("actionType");
         var eventParamsProp = commandsProp.FindPropertyRelative("eventParameters");
         var allowFadeoutProp = commandsProp.FindPropertyRelative("allowFadeout");

         EditorGUILayout.BeginVertical("box");
         
         commandsProp.isExpanded = EditorGUILayout.Foldout(commandsProp.isExpanded, $"Commands {i}", true);

         if (commandsProp.isExpanded)
         {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(actionTypeProp);

            if (actionTypeProp.enumValueIndex == 1)
            {
               EditorGUILayout.PropertyField(allowFadeoutProp);
            }

            if (actionTypeProp.enumValueIndex == 2)
            {
               EditorGUILayout.PropertyField(eventParamsProp, true);
            }
            EditorGUI.indentLevel--;
         }
         EditorGUILayout.EndVertical();
      }
      EditorGUILayout.Space();
      EditorGUILayout.BeginHorizontal();
      if (GUILayout.Button("Add"))
      {
         commandProp.arraySize++;
      }

      if (GUILayout.Button("Remove"))
      {
         commandProp.arraySize--;
      }
      EditorGUILayout.EndHorizontal();
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
         if (uecObjectsProp.arraySize > 0)
         {
            ShowCommands();
         }
      }
      else if (findMethodTypeProp.enumValueIndex == 1)
      {
         EditorGUILayout.LabelField("Find Object(s) with Tag(s)", EditorStyles.boldLabel);
         EditorGUILayout.PropertyField(objectTagsProp);
         if (objectTagsProp.arraySize > 0)
         {
            ShowCommands();
         }
      }
      
      serializedObject.ApplyModifiedProperties();
   }
}
