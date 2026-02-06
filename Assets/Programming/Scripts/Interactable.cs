using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private CommandWrapper[] commands;

    public void InteractRealTime(WorldAgent agent)
    {
        Command[] unwrappedCommands = commands.Select(c => c.UnwrapCommand(agent)).ToArray();
        agent.OverwriteQueue(unwrappedCommands);
    }

    public void InteractTurnBased(WorldAgent agent)
    {
        Command[] unwrappedCommands = commands.Select(c => c.UnwrapCommand(agent)).ToArray();
        agent.QueueCommands(unwrappedCommands);
    }
}