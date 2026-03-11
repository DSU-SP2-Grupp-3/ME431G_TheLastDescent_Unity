using System;
using System.Collections.Generic;
using System.Linq;
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
    private TMP_Text hitPointsText, actionPointsText, debuffHints;
    [SerializeField]
    private Image hitPointsImage, actionPointsImage, temperatureImage;

    [SerializeField]
    private GameObject statsContext;

    [SerializeField] private SettingsStorage storedSettings;

    private float maxHP;
    private float maxAP;

    private Locator<AgentManager> agentManager;

    private Locator<ModeSwitcher> modeSwitcher;

    private HashSet<WorldAgent.DebuffLevel> debuffLevels;

    private void Awake()
    {
        agentManager = new Locator<AgentManager>();

        modeSwitcher = new Locator<ModeSwitcher>();

        debuffLevels = new();
        debuffHints.text = "";
        
        player.OnDebuffApplied += AddDebuff;
        player.OnDebuffRemoved -= RemoveDebuff;
    }

    private void Start()
    {
        SetStatsVisbility(false);

        player.localStats.hitPoints.Changed += HitPointsChanged;
        player.localStats.actionPoints.Changed += ActionPointsChanged;
        player.localStats.temperature.Changed += TemperatureChanged;
        
        storedSettings.PlayerHpColorEvent += UpdateColors;
        storedSettings.PlayerApColorEvent += UpdateColors;
        storedSettings.PlayerHeatColorEvent += UpdateColors;
        // todo: add color event for debuff hints

        maxHP = player.localStats.initHitPoints;
        maxAP = player.localStats.initActionPoints;

        hitPointsText.text = $"HP: {player.localStats.hitPoints.value:0}/{maxHP:0}";
        actionPointsText.text = $"AP: {player.localStats.actionPoints.value:0.0}/{maxAP:0.0}";
    }

    public void ClickedOnPlayer()
    {
        agentManager.Get().SelectPlayer(player);
    }

    public void SetStatsVisbility(bool show)
    {
        if (modeSwitcher == null) return;
        if (!show && modeSwitcher.Get().mode == RoundClock.ProgressMode.TurnBased) statsContext.SetActive(true);
        else statsContext.SetActive(show);
    }

    public void OnHoverEnter()
    {
        agentManager.Get().SetPortraitAgent(player);
    }

    public void OnHoverExit()
    {
        agentManager.Get().SetPortraitAgent(null);
    }

    private void HitPointsChanged(float changed)
    {
        if (changed <= 0)
        {
            hitPointsText.text = $"HP: 0/{maxHP:0}";
            hitPointsImage.fillAmount = 0;
            button.interactable = false;
        }
        else
        {
            hitPointsText.text = $"HP: {changed:0}/{maxHP:0}";
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

    private void TemperatureChanged(float changed)
    {
        temperatureImage.fillAmount = changed;
    }

    private void AddDebuff(WorldAgent.DebuffLevel debuffLevel)
    {
        debuffLevels.Add(debuffLevel);
        UpdateDebuffHints();
    }

    private void RemoveDebuff(WorldAgent.DebuffLevel debuffLevel)
    {
        debuffLevels.Remove(debuffLevel);
        UpdateDebuffHints();
    }

    private void UpdateDebuffHints()
    {
        WorldAgent.DebuffLevel[] array = debuffLevels.ToArray();
        Array.Sort(array);
        debuffHints.text = "";
        foreach (WorldAgent.DebuffLevel debuffLevel in array)
        {
            debuffHints.text += $"{debuffLevel.debuff.hint}\n";
        }
    }
    
    private void UpdateColors()
    {
        hitPointsImage.color = storedSettings.PlayerHpColor;
        actionPointsImage.color = storedSettings.PlayerApColor;
        temperatureImage.color = storedSettings.PlayerHeatColor;
    }
}