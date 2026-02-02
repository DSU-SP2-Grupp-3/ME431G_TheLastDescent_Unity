using UnityEngine;

public class Locator<T> where T : Service<T>
{
    private T service;
    
    public Locator()
    {
        Locate();
    }

    private void Locate()
    {
        service = Service<T>.instance as T;
    }

    public T Get()
    {
        return service;
    }

    public bool TryGet(out T locatedService)
    {
        if (service)
        {
            locatedService = service;
            return true;
        }

        locatedService = null;
        return false;
    }
}
