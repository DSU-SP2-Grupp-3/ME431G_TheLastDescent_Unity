using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SaveManager : Service<SaveManager>
{
    [SerializeField]
    private string saveDataPath;
    private SaveData loadedData;

    [SerializeField]
    private bool nonplayerScene;
    public bool NonPlayerScene() => nonplayerScene;
    
    [SerializeField, HideIf("NonPlayerScene")]
    private WorldAgent playerA, playerB, playerC;

    [SerializeField, HideIf("NonPlayerScene")]
    private ResourceManager resourceManager;

    private SceneChanger sceneChanger;

    [SerializeField]
    private string[] levelNames;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        Register();
    }

    private void Start()
    {
        SceneChanger.OnRegister += sc =>
        {
            if (sceneChanger) sceneChanger.OnGoToScene -= SaveData;
            sceneChanger = sc;
            sceneChanger.OnGoToScene += SaveData;
        };
    }

    private void LoadData()
    {
        
        if (loadedData.newData) return;
        
        // set values
    }

    private void SaveData(string sceneName)
    {
        if (levelNames.Contains(sceneName)) loadedData.scene = sceneName;

        if (nonplayerScene) return;

        loadedData.playeyAHp = playerA.localStats.hitPoints;
        loadedData.playerATemp = playerA.localStats.temperature;
        
        loadedData.playeyBHp = playerB.localStats.hitPoints;
        loadedData.playerBTemp = playerB.localStats.temperature;
        
        loadedData.playeyCHp = playerC.localStats.hitPoints;
        loadedData.playerCTemp = playerC.localStats.temperature;

        loadedData.collectedResources = resourceManager.collectedResources;
    
    }

    public void GenerateExampleSaveData()
    {
        string path = $"Assets/Resources/Savedata/{saveDataPath}.json";

        loadedData = new();

        loadedData.playeyAHp = 10f;
        loadedData.playerATemp = 1f;
        
        loadedData.playeyBHp = 10f;
        loadedData.playerBTemp = 1f;
        
        loadedData.playeyCHp = 10f;
        loadedData.playerCTemp = 1f;

        loadedData.collectedResources = 5f;
        loadedData.scene = "Tutorial Level";

        StreamWriter writer = new StreamWriter(path, false);

        string json = JsonUtility.ToJson(loadedData, true);
        
        writer.Write(json);
        
        writer.Close();
        
        AssetDatabase.ImportAsset(path); 
    }

}
