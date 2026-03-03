using System;
using UnityEngine;

public class Watcher<T>
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

    public Watcher(T initialValue, Func<T, T> setFunction)
    {
        if (setFunction == null) SetFunction = (t) => t;
        else SetFunction = setFunction;
        value = initialValue;
    }

    public static implicit operator T(Watcher<T> w) => w.value;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{value} (watcher)";
    }
}