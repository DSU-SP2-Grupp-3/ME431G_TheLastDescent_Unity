using UnityEngine;

public class AI : MonoBehaviour
{
    [Header("Scriptable Objects")] 
    [SerializeField] private AIAttack attackBehaviour;
    [SerializeField] private AiMovement movementBehaviour;
    [SerializeField] private WorldAgent agent;
}
