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
        while (true)
        {
            int time = Random.Range(min, max);
            yield return new WaitForSeconds(time);

            if  (audioManager.IsPlaying(eventScriptable.eventReference))
            {
                continue;
            }

            int randIndex = Random.Range(0, randTracks.Length);
            int track = randTracks[randIndex];

            Debug.Log($"Playing Music track number {track}");

            EventPlayer player = audioManager.Getplayer(eventScriptable.eventReference.Guid);
            if (player == null)
            {
                audioManager.CreatePlayer(eventScriptable, out EventPlayer result);
                player = result;
            }

            player.RunInstanceModification(ParamName, track);
            player.PlayEvent();

        }
    }
}
