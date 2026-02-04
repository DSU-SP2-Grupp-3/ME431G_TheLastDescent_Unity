using System;
using UnityEngine;

// -se:
/// <summary>
/// A service is a globally accessible component. Components inheriting Service&lt;T> can be accessed globally through
/// a locator of the same type (i.e. Locator&lt;Clock> will access Clock). To be accessed Register() must be called
/// before. Only the instance that registered itself most recently will be accessed by the Locator.
/// </summary>
/// <typeparam name="T">The type of the component that is inheriting Service. For example: Clock : Service&lt;Clock></typeparam>
public abstract class Service<T> : MonoBehaviour
{
    public event Action<Service<T>> OnRegister;
    public event Action<Service<T>> OnDeregister; 
    
    public static Service<T> instance;
    protected void Register()
    {
        if (instance) OnDeregister?.Invoke(instance);
        instance = this;
        OnRegister?.Invoke(instance);
    }
    protected void Deregister()
    {
        if (instance) OnDeregister?.Invoke(instance);
        instance = null;
    }

}
