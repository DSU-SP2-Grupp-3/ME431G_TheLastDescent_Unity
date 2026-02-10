using UnityEngine;

public class DialogueCommandWrapper : CommandWrapper
{
    [SerializeField]
    private DialogueScriptable dialogueScriptable;
    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new DialogueCommand(dialogueScriptable, agent);
    }
}
