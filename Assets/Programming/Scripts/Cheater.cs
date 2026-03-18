using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Cheater : MonoBehaviour
{
    [SerializeField]
    private KeyCodeHook[] hooks;
    
    private void OnGUI()
    {
        if (!Debug.isDebugBuild) return;
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
        {
            OnKeyPressed(Event.current.keyCode);
        }
    }

    private void OnKeyPressed(KeyCode keyCode)
    {
        IEnumerable<KeyCodeHook> matching = hooks.Where(h => h.keyCode == keyCode);
        
        foreach (KeyCodeHook hook in matching)
        {
            hook.OnKeyPressed?.Invoke();
        }
    }

    [Serializable]
    private class KeyCodeHook
    {
        public KeyCode keyCode;
        public UnityEvent OnKeyPressed;
    }

    public void SetPlayerWalkSpeed(float speed)
    {
        List<WorldAgent> players = new Locator<AgentManager>().Get().GetPlayerAgents();
        foreach (WorldAgent player in players)
        {
            player.navMeshAgent.speed = speed;
        }
    }
    
    public void SetPlayerTurnSpeed(float angularSpeed)
    {
        List<WorldAgent> players = new Locator<AgentManager>().Get().GetPlayerAgents();
        foreach (WorldAgent player in players)
        {
            player.navMeshAgent.angularSpeed = angularSpeed;
        }
    }
    
    public void SetPlayerAcceleration(float acceleration)
    {
        List<WorldAgent> players = new Locator<AgentManager>().Get().GetPlayerAgents();
        foreach (WorldAgent player in players)
        {
            player.navMeshAgent.acceleration = acceleration;
        }
    }

    /// <summary>
    /// params take the form [Name of Agent]:[Name of transform object to set position to], ex: "PlayerA:pos1"
    /// </summary>
    public void SetAgentPosition(string parameters)
    {
        try
        {
            AgentManager am = new Locator<AgentManager>().Get();
            string[] split = parameters.Split(':');
            WorldAgent agent = am.GetAllAgents().First(p => p.gameObject.name == split[0]);
            Transform to = GameObject.Find(split[1]).transform;
            agent.navMeshAgent.Warp(to.position);
        }
        catch
        {
            Debug.LogWarning($"SetAgentPosition failed. Either: AgentManger is not registered, paramaters formatting is faulty, or the names given do not exist.");
        }
    }
}
