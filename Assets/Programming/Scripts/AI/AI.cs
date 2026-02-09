using UnityEngine;

public class AI : MonoBehaviour
{
    [Header("Scriptable Objects")] 
    [SerializeField] private AIAttack attackBehaviour;
    [SerializeField] private AIMovement movementBehaviour;
    [SerializeField] private WorldAgent agent;
    


    private void CreateCommands()
    {
        attackBehaviour.ProvideCommands();
    }
}
