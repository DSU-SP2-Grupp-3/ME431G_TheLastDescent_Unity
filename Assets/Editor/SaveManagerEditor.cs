using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    private SaveManager saveManager;

    public void OnEnable() 
    {
        saveManager = target as SaveManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate default file"))
        {
            saveManager.GenerateExampleSaveData();
        }
    }
}