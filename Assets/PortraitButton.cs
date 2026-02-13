using System;
using UnityEngine;
using UnityEngine.UI;

public class PortraitButton : MonoBehaviour
{
    [Tooltip("The player that should be selected")]
    [SerializeField] private WorldAgent player;
    [SerializeField] private Button button;
    
    private Locator<AgentManager> locatorAgentManager;
    private AgentManager agentManager;

    private void Start()
    {
        locatorAgentManager = new Locator<AgentManager>();
        agentManager = locatorAgentManager.Get();
        player.localStats.HitPointsChanged += Died;
    }

    public void ClickedOnPlayer()
    {
        agentManager.SelectPlayer(player);
    }

    private void Died(float changed)
    {
        if (player.localStats.hitPoints <= 0)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }
    
}
