using System;
using UnityEngine;

public class Watcher<T> where T : new()
{
    public event Action<T> Changed;

    private Func<T, T> SetFunction;

    private T _value;
    public T value
    {
        get => _value;
        set
        {
            _value = SetFunction(value);
            Changed?.Invoke(_value);
        }
    }

    public Watcher()
    {
        SetFunction = (t) => t;
        value = new T();
    }

    public Watcher(T initialValue, Func<T, T> setFunction)
    {
        if (setFunction == null) SetFunction = (t) => t;
        else SetFunction = setFunction;
        value = initialValue;
    }

    public static implicit operator T(Watcher<T> w) => w.value;

    /// <summary>
    /// Trigger the Changed event manually, should only be used if the value of T has been changed without assignment
    /// </summary>
    public void MarkChanged()
    {
        Changed?.Invoke(value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{value} (watcher)";
    }
}