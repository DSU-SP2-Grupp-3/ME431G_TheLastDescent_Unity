using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UniversalEventController))]
public class EditorUEC : Editor
{
    #region PropertyRegion
    //Boolean
    SerializedProperty playOnStartProp;
    SerializedProperty startWithParamProp;
    SerializedProperty allowFadeoutProp;
    SerializedProperty debugModeProp;
    //Enums
    SerializedProperty eventTypeSelect;
    SerializedProperty actionTypeSelect;
    SerializedProperty sfxTypeSelect;
    SerializedProperty musicEventSelect;
    SerializedProperty charEventSelect;
    SerializedProperty enemyEventSelect;
    SerializedProperty uiEventSelect;
    SerializedProperty interEventSelect;
    SerializedProperty ambienceEventSelect;
    //Array
    SerializedProperty Parameters;

    #endregion

    void OnEnable()
    {
        //Boolean
        playOnStartProp = serializedObject.FindProperty("playOnStart");
        startWithParamProp = serializedObject.FindProperty("startWithParameters");
        allowFadeoutProp = serializedObject.FindProperty("allowFadeout");
        debugModeProp = serializedObject.FindProperty("debugMode");
        //Enum
        eventTypeSelect = serializedObject.FindProperty("eventTypeSelect");
        actionTypeSelect = serializedObject.FindProperty("actionTypeSelect");
        sfxTypeSelect = serializedObject.FindProperty("sfxTypeSelect");
        musicEventSelect = serializedObject.FindProperty("musicEventSelect");
        charEventSelect = serializedObject.FindProperty("charEventSelect");
        enemyEventSelect = serializedObject.FindProperty("enemyEventSelect");
        uiEventSelect = serializedObject.FindProperty("uiEventSelect");
        interEventSelect = serializedObject.FindProperty("interEventSelect");
        ambienceEventSelect = serializedObject.FindProperty("ambienceEventSelect");
        //Array
        Parameters = serializedObject.FindProperty("parameters");

        

    }

    private void ShowActionMenu()
    {
        EditorGUILayout.PropertyField(actionTypeSelect);
        if (actionTypeSelect.enumValueIndex == 0) //Start
        {
            EditorGUILayout.PropertyField(playOnStartProp);
            EditorGUILayout.PropertyField(startWithParamProp);
            if (startWithParamProp.boolValue == true)
            {
                EditorGUILayout.PropertyField(Parameters, true);
            }
        }
        else if (actionTypeSelect.enumValueIndex == 1) //Stop
        {
            EditorGUILayout.PropertyField(allowFadeoutProp);
        }
        else if (actionTypeSelect.enumValueIndex == 2) //Set Parameter
        {
            EditorGUILayout.PropertyField(Parameters, true);
        }
        
    }

    private void ShowSfxTypeSelect()
    {
        EditorGUILayout.PropertyField(sfxTypeSelect);
        if (sfxTypeSelect.enumValueIndex == 0) //Character
        {
            EditorGUILayout.PropertyField(charEventSelect);
            ShowActionMenu();
        }
        else if (sfxTypeSelect.enumValueIndex == 1) //Enemy
        {
            EditorGUILayout.PropertyField(enemyEventSelect);
            ShowActionMenu();
        }
        else if (sfxTypeSelect.enumValueIndex == 2) //UI
        {
            EditorGUILayout.PropertyField(uiEventSelect);
            ShowActionMenu();
        }
        else if (sfxTypeSelect.enumValueIndex == 3) // Interactable
        {
            EditorGUILayout.PropertyField(interEventSelect);
            ShowActionMenu();
        }
    }

    public override void OnInspectorGUI()
    {
        //Content
        
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Select Event Type", EditorStyles.whiteBoldLabel);
        EditorGUILayout.PropertyField(eventTypeSelect);
        if (eventTypeSelect.enumValueIndex == 0) //Music
        {
            EditorGUILayout.LabelField("Select Music Event", EditorStyles.whiteBoldLabel);
            EditorGUILayout.PropertyField(musicEventSelect);
            ShowActionMenu();
        }
        else if (eventTypeSelect.enumValueIndex == 1) //Sfx
        {
            ShowSfxTypeSelect();
        }
        else if (eventTypeSelect.enumValueIndex == 2) //Ambiance
        {
            EditorGUILayout.PropertyField(ambienceEventSelect);
            ShowActionMenu();
        }
        
        EditorGUILayout.LabelField("Other Settings", EditorStyles.whiteBoldLabel);
        EditorGUILayout.PropertyField(debugModeProp);
        
        serializedObject.ApplyModifiedProperties();
        
    }
}
