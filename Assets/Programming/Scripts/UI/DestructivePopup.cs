using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DestructivePopup : MonoBehaviour
{
    public GameObject popPrefab;
    private GameObject temporaryGameObject;
    [SerializeField] DamageManager damageManager;

    private void Start()
    {
        damageManager.DealDamageEvent += SpawnPop;
    }

    public void SpawnPop(float popUpText, WorldAgent worldAgent)
    {
        temporaryGameObject = Instantiate(popPrefab, worldAgent.cameraFocusTransform.position + Vector3.up, Quaternion.identity, transform);
        temporaryGameObject.GetComponent<TextUpdater>().SetText(popUpText, worldAgent.cameraFocusTransform);
    }
}
