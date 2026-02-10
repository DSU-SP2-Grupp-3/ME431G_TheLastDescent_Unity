using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // todo: fix if two players interact with the same object on the same turn
    
    [SerializeField]
    private WorldAgent interactableAgent;
    [SerializeField]
    private CommandWrapper[] playerCommands, interactableCommands;
    
    public void InteractRealTime(WorldAgent agent)
    {
        Command[] unwrappedCommands = playerCommands.Select(c => c.UnwrapCommand(agent)).ToArray();
        agent.OverwriteQueue(unwrappedCommands); // agent is the invoking player/enemy/ally etc.
        agent.QueueCommand(new InvokeActionCommand(agent, () => QueueInteractablesCommand()));
        
    }

    public void InteractTurnBased(WorldAgent agent)
    {
        Command[] unwrappedCommands = playerCommands.Select(c => c.UnwrapCommand(agent)).ToArray();
        agent.QueueCommands(unwrappedCommands);
        agent.QueueCommand(new InvokeActionCommand(agent, () => QueueInteractablesCommand()));
    }

    private void QueueInteractablesCommand()
    {
        Command[] unwrappedCommands = UnwrapCommands(interactableCommands, interactableAgent);
        interactableAgent.QueueCommands(unwrappedCommands);
        interactableAgent.ForceStartCommandQueueExecution();
    }

    private Command[] UnwrapCommands(CommandWrapper[] wrappers, WorldAgent agent)
    {
        return wrappers.Select(c => c.UnwrapCommand(agent)).ToArray();
    }
}