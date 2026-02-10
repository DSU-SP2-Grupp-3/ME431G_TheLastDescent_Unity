using UnityEngine;

public class MoveCommandWrapper : CommandWrapper
{
    [SerializeField, Tooltip("Position relative to the game object that this component is attached to")]
    private Vector3 relativePosition;

    public override Command UnwrapCommand(WorldAgent agent)
    {
        Debug.Log(transform.position + relativePosition);
        return new MoveCommand(transform.position + relativePosition, agent);
    }
}