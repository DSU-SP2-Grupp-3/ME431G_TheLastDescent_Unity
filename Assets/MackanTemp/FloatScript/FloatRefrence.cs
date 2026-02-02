using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloatRefrence
{
    public bool useConstant = true;
    public float constantValue = 1;
    public FloatVariable fvar;

    public float Value
    {
        get => useConstant ? constantValue : fvar.Value;
        set
        {
            if (useConstant) constantValue = value;
            else fvar.Value = value;
        }
    }

    public static implicit operator float(FloatRefrence variable)
    {
        return variable.Value;
    }
}
