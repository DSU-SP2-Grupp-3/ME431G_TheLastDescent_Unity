using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private WorldAgent[] players;
    [SerializeField]
    private OrthographicCameraMover cameraMover;
    
    private Locator<InputManager> inputManager;

    private WorldAgent selectedPlayer;

    private void Start()
    {
        inputManager = new();
        inputManager.Get().ClickedOnPlayer += SelectPlayer;
        inputManager.Get().MovePlayerInput += MoveSelectedPlayerRealTime;
        SelectPlayer(players[0]);
    }

    private void SelectPlayer(WorldAgent playerAgent)
    {
        if (players.Contains(playerAgent))
        {
            selectedPlayer = playerAgent;
            cameraMover.targetGameObject = playerAgent.cameraFocusTransform;
            // todo: camera should move smoothly toward target transform and not follow animations on target -se
        }
    }

    private void MoveSelectedPlayerRealTime(Vector3 position)
    {
        MoveCommand movePlayer = new MoveCommand(position, selectedPlayer);
        if (movePlayer.possible) selectedPlayer.OverwriteCommand(movePlayer);
    }
    
    // undo command for selected player
    // attack enemy
    // 
    
}