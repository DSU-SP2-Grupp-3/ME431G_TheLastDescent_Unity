using UnityEngine;

public class MoveInRangeCommandWrapper : CommandWrapper
{
    [SerializeField, Tooltip("Position relative to the game object that this component is attached to")]
    private Vector3 relativePosition;

    [SerializeField, Tooltip("The range within which the character should move")]
    private float range;

    [SerializeField, Tooltip("The target agent with which line of sight must be achieved before stopping, " +
                             "leave empty if no such target is required.")]
    private WorldAgent lineOfSightTarget;
    
    public override Command UnwrapCommand(WorldAgent agent)
    {
        MoveInRangeCommand inRangeCommand = new MoveInRangeCommand(
            transform.position + relativePosition,
            range,
            agent,
            lineOfSightTarget
        );
        return inRangeCommand;
    }
}