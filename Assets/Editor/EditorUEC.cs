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
    SerializedProperty toggleReleaseProp;
    SerializedProperty positionStaticProp;
    SerializedProperty dbInitProp;
    SerializedProperty dbSETProp;
    SerializedProperty dbSATProp;
    SerializedProperty dbSSTProp;
    SerializedProperty dbSEPProp;
    SerializedProperty dbSEIProp;
    SerializedProperty dbPlayProp;
    SerializedProperty dbStopProp;
    SerializedProperty dbCreateProp;
    
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
        toggleReleaseProp = serializedObject.FindProperty("toggleRelease");
        positionStaticProp = serializedObject.FindProperty("positionStatic");
        debugModeProp = serializedObject.FindProperty("debugMode");
        
        dbInitProp = serializedObject.FindProperty("dbInit");
        dbSETProp = serializedObject.FindProperty("dbSET");
        dbSATProp = serializedObject.FindProperty("dbSAT");
        dbSSTProp = serializedObject.FindProperty("dbSST");
        dbSEPProp = serializedObject.FindProperty("dbSEP");
        dbSEIProp = serializedObject.FindProperty("dbSEI");
        dbPlayProp = serializedObject.FindProperty("dbPlay");
        dbStopProp = serializedObject.FindProperty("dbStop");
        dbCreateProp = serializedObject.FindProperty("dbCreate");
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
            EditorGUILayout.LabelField("Initiation Settings", EditorStyles.whiteBoldLabel);
            EditorGUILayout.PropertyField(playOnStartProp);
            EditorGUILayout.PropertyField(startWithParamProp);
            if (startWithParamProp.boolValue)
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
            EditorGUILayout.LabelField("Sfx Initiation Settings", EditorStyles.whiteBoldLabel);
            EditorGUILayout.PropertyField(toggleReleaseProp);
            EditorGUILayout.PropertyField(positionStaticProp);
            EditorGUILayout.PropertyField(playOnStartProp);
            EditorGUILayout.PropertyField(startWithParamProp);
            if (startWithParamProp.boolValue)
            {
                EditorGUILayout.PropertyField(Parameters, true);
            }
        }
        else if (sfxTypeSelect.enumValueIndex == 1) //Enemy
        {
            EditorGUILayout.PropertyField(enemyEventSelect);
            EditorGUILayout.LabelField("Sfx Initiation Settings", EditorStyles.whiteBoldLabel);
            EditorGUILayout.PropertyField(toggleReleaseProp);
            EditorGUILayout.PropertyField(positionStaticProp);
            EditorGUILayout.PropertyField(playOnStartProp);
            EditorGUILayout.PropertyField(startWithParamProp);
            if (startWithParamProp.boolValue)
            {
                EditorGUILayout.PropertyField(Parameters, true);
            }
        }
        else if (sfxTypeSelect.enumValueIndex == 2) //UI
        {
            EditorGUILayout.PropertyField(uiEventSelect);
            EditorGUILayout.LabelField("Sfx Initiation Settings", EditorStyles.whiteBoldLabel);
            EditorGUILayout.PropertyField(toggleReleaseProp);
            EditorGUILayout.PropertyField(positionStaticProp);
            EditorGUILayout.PropertyField(playOnStartProp);
            EditorGUILayout.PropertyField(startWithParamProp);
            if (startWithParamProp.boolValue)
            {
                EditorGUILayout.PropertyField(Parameters, true);
            }
        }
        else if (sfxTypeSelect.enumValueIndex == 3) // Interactable
        {
            EditorGUILayout.PropertyField(interEventSelect);
            EditorGUILayout.LabelField("Sfx Initiation Settings", EditorStyles.whiteBoldLabel);
            EditorGUILayout.PropertyField(toggleReleaseProp);
            EditorGUILayout.PropertyField(positionStaticProp);
            EditorGUILayout.PropertyField(playOnStartProp);
            EditorGUILayout.PropertyField(startWithParamProp);
            if (startWithParamProp.boolValue)
            {
                EditorGUILayout.PropertyField(Parameters, true);
            }
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
            EditorGUILayout.LabelField("Select Sfx Type", EditorStyles.whiteBoldLabel);
            ShowSfxTypeSelect();
        }
        else if (eventTypeSelect.enumValueIndex == 2) //Ambiance
        {
            EditorGUILayout.LabelField("Select Ambiance Event", EditorStyles.whiteBoldLabel);
            EditorGUILayout.PropertyField(ambienceEventSelect);
            ShowActionMenu();
        }
        
        EditorGUILayout.LabelField("Other Settings", EditorStyles.whiteBoldLabel);
        EditorGUILayout.PropertyField(debugModeProp);
        if (debugModeProp.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(dbInitProp);
            EditorGUILayout.PropertyField(dbSETProp);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(dbSATProp);
            EditorGUILayout.PropertyField(dbSSTProp);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(dbSEPProp);
            EditorGUILayout.PropertyField(dbSEIProp);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(dbPlayProp);
            EditorGUILayout.PropertyField(dbStopProp);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(dbCreateProp);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Creamy Thighs", MessageType.Info);

        }
        
        serializedObject.ApplyModifiedProperties();
        
    }
}
