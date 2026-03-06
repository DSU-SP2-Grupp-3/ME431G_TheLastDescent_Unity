using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private SettingsStorage storedSettings;
    public static AudioSettings Instance { get; private set; }

    [Header("Volume")] 
    [Range(0f, 1f)] 
    public float masterVolume;
    [Range(0f, 1f)]
    public float musicVolume;
    [Range(0f, 1f)]
    public float effectVolume;
    [Range(0f,1f)]
    public float ambienceVolume;
    [Range(0f,1f)]
    public float dialogueVolume;
    
    [Header("Paths")]
    [SerializeField] private string masterVCAPath;
    [SerializeField] private string musicVCAPath;
    [SerializeField] private string effectVCAPath;
    [SerializeField] private string ambienceVCAPath;
    [SerializeField] private string dialogueVCAPath;
    
    private VCA masterVCA;
    private VCA musicVCA;
    private VCA effectVCA;
    private VCA ambienceVCA;
    private VCA dialogueVCA;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        masterVolume = storedSettings.masterVolume;
        effectVolume = storedSettings.effectVolume;
        musicVolume = storedSettings.musicVolume;
        ambienceVolume = storedSettings.ambienceVolume;
        dialogueVolume = storedSettings.dialogueVolume;
        
        masterVCA = RuntimeManager.GetVCA(masterVCAPath);
        effectVCA = RuntimeManager.GetVCA(effectVCAPath);
        musicVCA = RuntimeManager.GetVCA(musicVCAPath);
        dialogueVCA = RuntimeManager.GetVCA(dialogueVCAPath);
        ambienceVCA = RuntimeManager.GetVCA(ambienceVCAPath);
    }

    // Update is called once per frame
    public void UpdateVolumes()
    {
        storedSettings.masterVolume = masterVolume;
        storedSettings.effectVolume = effectVolume;
        storedSettings.musicVolume = musicVolume;
        storedSettings.ambienceVolume = ambienceVolume;
        storedSettings.dialogueVolume = dialogueVolume;
    }

    public void SetVolumes()
    {
        masterVCA.setVolume(storedSettings.masterVolume);
        effectVCA.setVolume(storedSettings.effectVolume);
        musicVCA.setVolume(storedSettings.musicVolume);
        ambienceVCA.setVolume(storedSettings.ambienceVolume);
        dialogueVCA.setVolume(storedSettings.dialogueVolume);
    }
}
