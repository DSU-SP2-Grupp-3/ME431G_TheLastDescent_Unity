using System;
using System.Collections.Generic;
using FMODUnity;
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
    
    //Array
    SerializedProperty Parameters;
    
    //Integers
    SerializedProperty musicIndex;
    SerializedProperty ambIndex;
    SerializedProperty charIndex;
    SerializedProperty enemyIndex;
    SerializedProperty uiIndex;
    SerializedProperty interIndex;
    
    List<String> musicEventSelectList = new List<String>();
    List<String> charEventSelectList = new List<String>();
    List<String> enemyEventSelectList = new List<String>();
    List<String> uiEventSelectList = new List<String>();
    List<String> interEventSelectList = new List<String>();
    List<String> ambienceEventSelectList = new List<String>();
    
    

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
        //Array
        Parameters = serializedObject.FindProperty("parameters");
        //Integers
        musicIndex = serializedObject.FindProperty("muIndex");
        ambIndex = serializedObject.FindProperty("ambIndex");
        charIndex = serializedObject.FindProperty("charIndex");
        enemyIndex = serializedObject.FindProperty("enemyIndex");
        uiIndex = serializedObject.FindProperty("uiIndex");
        interIndex = serializedObject.FindProperty("interIndex");
        
        UpdateEventSelectionSubs();


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
            charIndex.intValue = EditorGUILayout.Popup("Character Event", charIndex.intValue, charEventSelectList.ToArray());
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
            enemyIndex.intValue = EditorGUILayout.Popup("Enemy Event", enemyIndex.intValue, enemyEventSelectList.ToArray());
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
            uiIndex.intValue = EditorGUILayout.Popup("UI Event", uiIndex.intValue, uiEventSelectList.ToArray());
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
            interIndex.intValue = EditorGUILayout.Popup("Interactables Event", interIndex.intValue, interEventSelectList.ToArray());
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
            musicIndex.intValue = EditorGUILayout.Popup("Music Event", musicIndex.intValue, musicEventSelectList.ToArray());

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
            ambIndex.intValue = EditorGUILayout.Popup("Music Event", ambIndex.intValue, ambienceEventSelectList.ToArray());

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

        }
        
        serializedObject.ApplyModifiedProperties();
        
    }
    
    private void UpdateEventSelectionSubs()
    {
        var uec = (UniversalEventController)target;
        var events = FMODUnity.EventManager.Events;
        foreach (var x in events)
        {
            EventReference eventRef = new EventReference();
            
            eventRef.Path = x.Path;
            if (x.Path.Contains("/Music/"))
            {
                string[] muSubs = x.Path.Split("/");
                musicEventSelectList.Add(muSubs[^1]);
            }
            else if (x.Path.Contains("/AMB/"))
            {
                string[] ambSubs = x.Path.Split("/");
                ambienceEventSelectList.Add(ambSubs[^1]);
            }
            else if (x.Path.Contains("/Character/"))
            {
                string[] characterSubs = x.Path.Split("/");
                charEventSelectList.Add(characterSubs[^1]);
            }
            else if (x.Path.Contains("/Enemies/"))
            {
                string[] enemySubs = x.Path.Split("/");
                enemyEventSelectList.Add(enemySubs[^1]);
            }
            else if (x.Path.Contains("/UI/"))
            {
                string[] uiSubs = x.Path.Split("/");
                uiEventSelectList.Add(uiSubs[^1]);
            }
            else if (x.Path.Contains("/Interactables/"))
            {
                string[] interactSubs = x.Path.Split("/");
                interEventSelectList.Add(interactSubs[^1]);
            }
            
        }
        //Något fuffens här 
        uec.musicEventSelectList = musicEventSelectList;
        uec.ambienceEventSelectList = ambienceEventSelectList;
        uec.charEventSelectList = charEventSelectList;
        uec.enemyEventSelectList = enemyEventSelectList;
        uec.uiEventSelectList = uiEventSelectList;
        uec.interEventSelectList = interEventSelectList;
    }
}
