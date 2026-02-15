using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioSettings : MonoBehaviour
{
    public static AudioSettings Instance { get; private set; }
    
    [Header("Volume")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float effectVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [SerializeField] private string masterVCAPath;
    [SerializeField] private string effectVCAPath;
    [SerializeField] private string musicVCAPath;
    
    private VCA masterVCA;
    private VCA effectVCA;
    private VCA musicVCA;
    
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
        effectVCA = RuntimeManager.GetVCA(effectVCAPath);
        masterVCA = RuntimeManager.GetVCA(masterVCAPath);
        musicVCA = RuntimeManager.GetVCA(musicVCAPath);
    }

    // Update is called once per frame
    void Update()
    {
        masterVCA.setVolume(masterVolume);
        effectVCA.setVolume(effectVolume);
        musicVCA.setVolume(musicVolume);
    }
}
