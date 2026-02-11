using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioManager))]
public class EditorAudioManager : Editor
{
   #region Properties

   SerializedProperty muProp;
   SerializedProperty ambProp;
   SerializedProperty charProp;
   SerializedProperty enemyProp;
   SerializedProperty uiProp;
   SerializedProperty interProp;

   #endregion

   public void OnEnable()
   {
      
      
      muProp = serializedObject.FindProperty("muRef");
      ambProp = serializedObject.FindProperty("ambRef");
      charProp = serializedObject.FindProperty("characterRef");
      enemyProp = serializedObject.FindProperty("enemyRef");
      uiProp = serializedObject.FindProperty("uiRef");
      interProp = serializedObject.FindProperty("interactRef");
      
   }

   public override void OnInspectorGUI()
   {
      AudioManager audioManager = (AudioManager)target;
      //Content
      serializedObject.Update();
      
      EditorGUILayout.PropertyField(muProp);
      EditorGUILayout.PropertyField(ambProp);
      EditorGUILayout.PropertyField(charProp);
      EditorGUILayout.PropertyField(enemyProp);
      EditorGUILayout.PropertyField(uiProp);
      EditorGUILayout.PropertyField(interProp);

      EditorGUILayout.BeginHorizontal();

      if (GUILayout.Button("Update Event References"))
      {
         audioManager.UpdateReferences();
      }
      
      EditorGUILayout.EndHorizontal();
      
      
      serializedObject.ApplyModifiedProperties();
   }
}
