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

    private void OnDestroy()
    {
        damageManager.DealDamageEvent -= SpawnPop;
    }

    public void SpawnPop(float popUpText, WorldAgent worldAgent)
    {
        temporaryGameObject = Instantiate(popPrefab, transform.position, Quaternion.identity, transform);
        temporaryTextUpdater = temporaryGameObject.GetComponent<TextUpdater>();
        temporaryTextUpdater.Target = worldAgent.cameraFocusTransform.position;
        temporaryTextUpdater.SetText(popUpText);
    }
}
