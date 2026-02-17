using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class CinematicKitService : Service<CinematicKitService>
{
    public List<WorldAgent> Actors;
    void Awake()
    {
        Register();
    }
    /// <summary>
    /// -Ma.
    /// Finds and registers actors to later be called for cinematic purposes.
    /// We use int IDs at the moment to make a connection, but this may be changed later on.
    /// </summary>
    /// <param name="IDs"></param>
    public bool FindActors(int[] IDs)
    {

        Debug.Log("looking");
        List<WorldAgent> agents = new Locator<AgentManager>().Get().GetAllAgents();
        foreach (WorldAgent agent in agents)
        {
            for (int i = 0; i < IDs.Length; i++)
            {
                if (IDs[i] == agent.actorID)
                {
                    RegisterActor(agent);
                }
            }
        }
        return true;
    }
    private void RegisterActor(WorldAgent worldAgent)
    {
        Debug.Log("Registered");
        Actors.Add(worldAgent);
    }
    private void UnregisterActor(WorldAgent worldAgent)
    {
        Actors.Remove(worldAgent);
    }
    public void ClearCinematicScene()
    {
        Actors.Clear();
    }
    public WorldAgent GetActor(int id)
    {
        return Actors.FirstOrDefault(a => a.actorID == id);  
    }

}
