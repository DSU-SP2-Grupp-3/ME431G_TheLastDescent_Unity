using UnityEngine;

public class MoveInRangeCommandWrapper : CommandWrapper
{
    [SerializeField, Tooltip("Position relative to the game object that this component is attached to")]
    private Vector3 relativePosition;

    [SerializeField, Tooltip("The range within which the character should move")]
    private float range;

    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new MoveInRangeCommand(transform.position + relativePosition, range, agent);
    }
}