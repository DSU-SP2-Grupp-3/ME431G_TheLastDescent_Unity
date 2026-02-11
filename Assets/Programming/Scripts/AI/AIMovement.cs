using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AiMovement", menuName = "Scriptable Objects/AiMovement")]
public class AIMovement : ScriptableObject, CommandProvider
{
    private Locator<AgentManager> agentManager;

    private void OnEnable()
    {
        agentManager = new();
    }

    /// <inheritdoc />
    public Command[] ProvideCommands()
    {

        return Array.Empty<Command>();
    }
}
