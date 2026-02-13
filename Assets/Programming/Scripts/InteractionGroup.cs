using System;
using Unity.Mathematics;
using UnityEngine;

public class InteractionGroup : MonoBehaviour
{
    [SerializeField]
    private Interactable[] interactables;
    private int interactionIndex;

    public void RemoteInteraction()
    {
        interactables[interactionIndex].InteractRemotely();
    }

    public void SetInteractionIndex(int index)
    {
        interactionIndex = Math.Max(index, 0);
    }

    public void UnwrapInteractableCommands(WorldAgent playerAgent, out Interactable.InteractionResult result)
    {
        interactables[interactionIndex].UnwrapInteractableCommands(playerAgent, out result);
    }
}