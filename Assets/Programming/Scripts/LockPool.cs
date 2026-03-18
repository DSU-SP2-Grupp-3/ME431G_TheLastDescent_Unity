using System.Collections.Generic;

public class LockPool
{
    private HashSet<object> locks;

    public LockPool()
    {
        locks = new();
    }

    public void Lock(object objectLock)
    {
        locks.Add(objectLock);
    }

    public void Unlock(object objectLock)
    {
        locks.Remove(objectLock);
    }

    public static implicit operator bool(LockPool lp)
    {
        // UnityEngine.Debug.Log($"{nameof(lp)}: {lp.locks.Count}");
        return lp.locks.Count == 0;
    }
}