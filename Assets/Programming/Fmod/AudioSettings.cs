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
    public float ambianceVolume;
    [Range(0f,1f)]
    public float dialogVolume;
    

    [SerializeField] private string masterVCAPath;
    [SerializeField] private string effectVCAPath;
    [SerializeField] private string musicVCAPath;
    [SerializeField] private string ambianceVCAPath;
    [SerializeField] private string dialogVCAPath;
    
    private VCA masterVCA;
    private VCA effectVCA;
    private VCA musicVCA;
    private VCA ambianceVCA;
    private VCA dialogVCA;
    
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
        ambianceVCA = RuntimeManager.GetVCA(ambianceVCAPath);
        dialogVCA = RuntimeManager.GetVCA(dialogVCAPath);
    }

    // Update is called once per frame
    void Update()
    {
        masterVCA.setVolume(masterVolume);
        effectVCA.setVolume(effectVolume);
        musicVCA.setVolume(musicVolume);
        ambianceVCA.setVolume(ambianceVolume);
        dialogVCA.setVolume(dialogVolume);
    }
}
