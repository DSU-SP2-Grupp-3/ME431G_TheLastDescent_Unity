using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioSettings : MonoBehaviour
{
    public static AudioSettings Instance { get; private set; }

    [Header("Volume")] [Range(0f, 1f)] 
    public float masterVolume;
    [Range(0f, 1f)]
    public float effectVolume;
    [Range(0f, 1f)]
    public float musicVolume;
    [Range(0f,1f)]
    public float ambienceVolume;
    [Range(0f,1f)]
    public float dialogueVolume;
    

    [SerializeField] private string masterVCAPath;
    [SerializeField] private string effectVCAPath;
    [SerializeField] private string musicVCAPath;
    [SerializeField] private string ambienceVCAPath;
    [SerializeField] private string dialogueVCAPath;
    
    private VCA masterVCA;
    private VCA effectVCA;
    private VCA musicVCA;
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
        masterVCA = RuntimeManager.GetVCA(masterVCAPath);
        effectVCA = RuntimeManager.GetVCA(effectVCAPath);
        musicVCA = RuntimeManager.GetVCA(musicVCAPath);
        ambienceVCA = RuntimeManager.GetVCA(ambienceVCAPath);
        dialogueVCA = RuntimeManager.GetVCA(dialogueVCAPath);
    }

    // Update is called once per frame
    void Update()
    {
        masterVCA.setVolume(masterVolume);
        effectVCA.setVolume(effectVolume);
        musicVCA.setVolume(musicVolume);
        ambienceVCA.setVolume(ambienceVolume);
        dialogueVCA.setVolume(dialogueVolume);
    }
}
