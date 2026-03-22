using System;

[Serializable]
public class SaveData
{
    public bool newData;
    
    public float playeyAHp;
    public float playerATemp;

    public float playeyBHp;
    public float playerBTemp;

    public float playeyCHp;
    public float playerCTemp;

    public float collectedResources;

    public string scene;

    // todo: settings need to be saved as well
}