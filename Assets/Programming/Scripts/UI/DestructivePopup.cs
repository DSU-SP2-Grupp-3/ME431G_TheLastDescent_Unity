using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DestructivePopup : MonoBehaviour
{
    public GameObject popPrefab;
    private GameObject temporaryGameObject;
    private TextUpdater temporaryTextUpdater;
    [SerializeField] DamageManager damageManager;

    private void Start()
    {
        damageManager.DealDamageEvent += SpawnPop;
    }

    public void SpawnPop(float popUpText, WorldAgent worldAgent)
    {
        temporaryGameObject = Instantiate(popPrefab, worldAgent.transform.position + Vector3.up, Quaternion.identity, transform);
        temporaryTextUpdater = temporaryGameObject.GetComponent<TextUpdater>();
        temporaryTextUpdater.SetText(popUpText);
    }
}
