using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    private List<WorldAgent> enemies;

    private void Start()
    {
        enemies = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<WorldAgent>(out WorldAgent agent))
            {
                if (agent.team == WorldAgent.Team.Enemy)
                {
                    enemies.Add(agent);
                    agent.ForcedEnterTurnBased += OnEnemyForceEnterTurnBased;
                }
            }
        }
    }

    private void OnEnemyForceEnterTurnBased(WorldAgent triggerer)
    {
        Debug.Log($"Enter {enemies.Count} enemies into combat");
        foreach (WorldAgent enemy in enemies)
        {
            enemy.ForcedEnterTurnBased -= OnEnemyForceEnterTurnBased;
            if (enemy != triggerer)
            {
                enemy.Activate();
            }
        }
    }
}
