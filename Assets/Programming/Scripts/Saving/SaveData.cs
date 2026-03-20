using System;

[Serializable]
public struct SaveData
{
    public float playeyAHp;
    public float playerATemp;

    public float playeyBHp;
    public float playerBTemp;

    public float playeyCHp;
    public float playerCTemp;

    public float collectedResources;

    public string lastLoadedScene;

    // todo: settings need to be saved as well
}