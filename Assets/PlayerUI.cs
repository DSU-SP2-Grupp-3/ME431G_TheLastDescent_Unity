using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Tooltip("The player that should be selected")]
    [SerializeField] private WorldAgent player;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text hitPointsText;
    [SerializeField] private TMP_Text actionPointsText;

    private float maxHP;
    private float maxAP;

    private Locator<AgentManager> locatorAgentManager;
    private AgentManager agentManager;

    private void Start()
    {
        locatorAgentManager = new Locator<AgentManager>();
        agentManager = locatorAgentManager.Get();
        
        player.localStats.HitPointsChanged += HitPointsChanged;
        player.localStats.ActionPointsChanged += ActionPointsChanged;
        
        maxHP = player.localStats.initHitPoints > player.localStats.hitPoints ? player.localStats.initHitPoints : player.localStats.hitPoints;
        maxAP = player.localStats.initActionPoints > player.localStats.actionPoints ? player.localStats.initActionPoints : player.localStats.actionPoints;

        
        hitPointsText.text = $"HP: {player.localStats.hitPoints}/{maxHP}";
        actionPointsText.text = $"AP: {player.localStats.actionPoints}/{player.localStats.initActionPoints}";
    }

    public void ClickedOnPlayer()
    {
        player.localStats.hitPoints -= 1;
        agentManager.SelectPlayer(player);
    }

    private void HitPointsChanged(float changed)
    {
        if (player.localStats.hitPoints <= 0)
        {
            hitPointsText.text = $"HP: 0/{maxHP}";

            button.interactable = false;
        }
        else
        {
            hitPointsText.text = $"HP: {player.localStats.hitPoints}/{maxHP}";
            button.interactable = true;
        }
        Debug.Log(player.localStats.hitPoints);
    }

    private void ActionPointsChanged(float changed)
    {
        actionPointsText.text = $"AP: {player.localStats.actionPoints}/{maxAP}";
    }

    
}
