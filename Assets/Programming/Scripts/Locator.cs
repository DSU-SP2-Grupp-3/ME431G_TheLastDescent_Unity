using UnityEngine;

// -se:
/// <summary>
/// A locator provides a reference to a component inheriting Service&lt;T>. For example: Locator&lt;Clock>.Get() returns
/// a reference to the latest registered Clock instance or null if no Clock instance has registered itself or is active.
/// </summary>
/// <typeparam name="T">The type of the component inheriting Service&lt;T></typeparam>
public class Locator<T> where T : Service<T>
{
    private T service;

    private void Locate()
    {
        service = Service<T>.instance as T;
    }

    public T Get()
    {
        Locate();
        return service;
    }

    public bool TryGet(out T locatedService)
    {
        Locate();
        if (service)
        {
            locatedService = service;
            return true;
        }

        locatedService = null;
        return false;
    }
}