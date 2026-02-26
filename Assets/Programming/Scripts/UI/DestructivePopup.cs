using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DestructivePopup : MonoBehaviour
{
    public GameObject popPrefab;
    private GameObject temporaryGameObject;
    private TextUpdater temporaryTextUpdater;
    public void SpawnPop(string popUpText, Vector3 position)
    {
        temporaryGameObject = Instantiate(popPrefab, position, Quaternion.identity, transform);
        temporaryTextUpdater = temporaryGameObject.GetComponent<TextUpdater>();
        temporaryTextUpdater.SetText(popUpText);
    }
}
