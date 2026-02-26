using System.Linq;
using UnityEngine;

public class EventCollection : Service<EventCollection>
{
    /*
    [SerializeField]
    private SimpleEventPlayer[] eventPlayers;

    private void Awake()
    {
        Register();
    }

    public void PlayEvent(string eventName)
    {
        SimpleEventPlayer player = eventPlayers.FirstOrDefault(p => p.eventName == eventName);
        if (player)
        {
            player.PlayEvent();
        }
    }
    */
}