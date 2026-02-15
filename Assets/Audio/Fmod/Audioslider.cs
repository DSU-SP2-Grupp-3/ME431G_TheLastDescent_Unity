using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    private enum VolumeType
    {
        MASTER,
        EFFECT,
        MUSIC,
    }

    [Header("AudioType")] 
    [SerializeField] private VolumeType volumeType;
    
    private Slider volumeSlider;
    
    void Awake()
    {
        volumeSlider = this.GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                volumeSlider.value = AudioSettings.Instance.masterVolume;
                break;
            case VolumeType.EFFECT:
                volumeSlider.value = AudioSettings.Instance.effectVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = AudioSettings.Instance.musicVolume;
                break;
            default:
                Debug.LogWarning("Volume type not supported: " + volumeType);
                break;
        }
    }

    public void onSliderChange()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                AudioSettings.Instance.masterVolume = volumeSlider.value;
                break;
            case VolumeType.EFFECT:
                AudioSettings.Instance.effectVolume = volumeSlider.value;
                break;
            case VolumeType.MUSIC:
                AudioSettings.Instance.musicVolume = volumeSlider.value;
                break;
            default:
                Debug.LogWarning("Volume type not supported: " + volumeType);
                break;
        }
    }
}