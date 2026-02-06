using System;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private WorldAgent[] players;
    [SerializeField]
    private OrthographicCameraMover cameraMover;

    private Locator<InputManager> inputManager;
    private Locator<ModeSwitcher> modeSwitcher;

    private WorldAgent selectedPlayer;

    private void Start()
    {
        inputManager = new();
        modeSwitcher = new();
        inputManager.Get().ClickedOnPlayer += SelectPlayer;
        inputManager.Get().MovePlayerInput += MoveSelectedPlayer;
        inputManager.Get().ClickedEnvironment += ClickedEnvironment;
        SelectPlayer(players[0]);
    }

    private void SelectPlayer(WorldAgent playerAgent)
    {
        if (players.Contains(playerAgent))
        {
            // Debug.Log($"Select {playerAgent.name}");
            selectedPlayer = playerAgent;
            cameraMover.targetGameObject = playerAgent.cameraFocusTransform;
            // todo: camera should move smoothly toward target transform and not follow animations on target -se
        }
    }

    private void MoveSelectedPlayer(Vector3 position)
    {
        MoveCommand movePlayer = new MoveCommand(position, selectedPlayer);
        if (!movePlayer.possible) return;

        RealTimeOrTurnBased(
            () => selectedPlayer.OverwriteQueue(movePlayer),
            () => selectedPlayer.QueueCommand(movePlayer)
        );
    }

    private void ClickedEnvironment(GameObject go)
    {
        if (go.TryGetComponent<Interactable>(out Interactable interactable))
        {
            RealTimeOrTurnBased(
                () => interactable.InteractRealTime(selectedPlayer),
                () => interactable.InteractTurnBased(selectedPlayer)
            );
        }
    }
    // undo command for selected player
    // attack enemy
    //

    private void RealTimeOrTurnBased(Action realTime, Action turnBased)
    {
        switch (modeSwitcher.Get().mode)
        {
            case RoundClock.ProgressMode.RealTime:
                realTime.Invoke();
                break;
            case RoundClock.ProgressMode.TurnBased:
                turnBased.Invoke();
                break;
        }
    }
}