using UnityEngine;

public class Resource : MonoBehaviour
{
    public float amount;

    public void Collect()
    {
        gameObject.SetActive(false);
    }
}
