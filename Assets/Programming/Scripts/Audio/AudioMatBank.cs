using UnityEngine;

public class AudioMatBank : MonoBehaviour
{
    public AudioMat[] audioMats;

    public EventScriptable GetSoundFromMat(Material material)
    {
        for (int i = 0; i < audioMats.Length; i++)
        {
            if (audioMats[i].mat == material)
            {
                return audioMats[i].eventScriptable;
            }
        }
        return null;
    }
}
