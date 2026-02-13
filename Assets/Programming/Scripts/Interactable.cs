using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // todo: fix if two players interact with the same object on the same turn

    [SerializeField]
    protected WorldAgent interactableAgent;
    [SerializeField]
    protected CommandWrapper[] playerCommands, interactableCommands;

    public void InteractRemotely()
    {
        QueueInteractablesCommand();
    }

    public void UnwrapInteractableCommands(WorldAgent playerAgent, out InteractionResult result)
    {
        Command[] unwrappedPlayerCommands = UnwrapCommands(playerCommands, playerAgent);
        Command[] unwrappedInteractableCommands = UnwrapCommands(interactableCommands, interactableAgent);
        result = new();
        result.interactableAgent = interactableAgent;
        result.interactableAgentCommands = unwrappedInteractableCommands;
        result.invokingAgentCommands = unwrappedPlayerCommands;
    }

    private void QueueInteractablesCommand()
    {
        Command[] unwrappedCommands = UnwrapCommands(interactableCommands, interactableAgent);
        interactableAgent.QueueCommands(unwrappedCommands);
        interactableAgent.ForceStartCommandQueueExecution();
    }

    protected Command[] UnwrapCommands(CommandWrapper[] wrappers, WorldAgent agent)
    {
        return wrappers.Select(c => c.UnwrapCommand(agent)).ToArray();
    }

    public class InteractionResult
    {
        public WorldAgent interactableAgent;
        public Command[] interactableAgentCommands;
        public Command[] invokingAgentCommands;

        public InvokeActionCommand QueueInteractablesCommand(WorldAgent playerAgent)
        {
            return new InvokeActionCommand(playerAgent, () =>
            {
                interactableAgent.QueueCommands(interactableAgentCommands);
                interactableAgent.ForceStartCommandQueueExecution();
            });
        }
    }
}