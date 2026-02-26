using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Tooltip("The player that should be selected")]
    [SerializeField]
    private WorldAgent player;
    [SerializeField]
    private Button button;
    [SerializeField]
    private TMP_Text hitPointsText;
    [SerializeField]
    private TMP_Text actionPointsText;
    [SerializeField]
    private Image hitPointsImage;
    [SerializeField]
    private Image actionPointsImage;

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

        maxHP = player.localStats.initHitPoints;
        maxAP = player.localStats.initActionPoints;

        hitPointsText.text = $"HP: {player.localStats.hitPoints:0}/{maxHP:0}";
        actionPointsText.text = $"AP: {player.localStats.actionPoints:0.0}/{maxAP:0.0}";
    }

    public void ClickedOnPlayer()
    {
        agentManager.SelectPlayer(player);
    }

    private void HitPointsChanged(float changed)
    {
        if (changed <= 0)
        {
            hitPointsText.text = $"HP: 0/{(int)maxHP}";
            hitPointsImage.fillAmount = 0;
            button.interactable = false;
        }
        else
        {
            hitPointsText.text = $"HP: {changed:0}/{changed:0}";
            hitPointsImage.fillAmount = changed / maxHP;
            button.interactable = true;
        }
    }

    private void ActionPointsChanged(float changed)
    {
        if (changed <= 0.05f)
        {
            actionPointsText.text = $"AP: 0.0/{maxAP:0.0}";
            actionPointsImage.fillAmount = 0;
        }
        else
        {
            actionPointsText.text = $"AP: {changed:0.0}/{maxAP:0.0}";
            actionPointsImage.fillAmount = changed / maxAP;
        }
    }
}