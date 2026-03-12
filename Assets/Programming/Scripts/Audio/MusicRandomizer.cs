using System.Collections;
using FMODUnity;
using UnityEngine;

public class MusicRandomizer : MonoBehaviour
{
    public string ParamName;
    public EventScriptable eventScriptable;
    private AudioManager audioManager;
    public int[] randTracks;
    public int min;
    public int max;
    public void Start()
    {
        audioManager = new Locator<AudioManager>().Get();
        StartCoroutine(TryToPlayMusic());
    }
    private IEnumerator TryToPlayMusic()
    {
        //Does not work for some reason, will fix after speltest 2
        while (true)
        {
            if (audioManager.IsPlaying(eventScriptable.eventReference))
            {
                int time = Random.Range(min, max);
                yield return new WaitForSeconds(time);
                int RandTrack = Random.Range(0, randTracks.Length - 1);
                int Track = randTracks[RandTrack];
                audioManager.PlayAudioEvent(eventScriptable);
                audioManager.RunInstanceModification(eventScriptable, ParamName, Track);
            }
            yield return new WaitForSeconds(120);
        }
    }
}
