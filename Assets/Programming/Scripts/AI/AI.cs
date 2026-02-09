using UnityEngine;

public class AI : MonoBehaviour
{
    [Header("Scriptable Objects")] 
    [SerializeField] private AiAttack attackBehaviour;
    [SerializeField] private AiMovement movementBehaviour;
    [SerializeField] private WorldAgent agent;
    


    private void CreateCommands()
    {
        attackBehaviour.ProvideCommands();
    }
}
