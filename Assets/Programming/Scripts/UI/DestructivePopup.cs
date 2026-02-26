using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DestructivePopup : MonoBehaviour
{
    public GameObject popPrefab; 
    
    public void SpawnPop(string popUpText, Vector3 position)
    {
        popPrefab.GetComponent<TextUpdater>()
    }
}
