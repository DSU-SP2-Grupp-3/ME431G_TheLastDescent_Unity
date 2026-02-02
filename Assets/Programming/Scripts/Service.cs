using UnityEngine;

public abstract class Service<T> : MonoBehaviour
{
    public static Service<T> instance;
    protected void Register()
    {
        instance = this;
    }

}
