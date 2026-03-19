using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class HideIfAttribute : PropertyAttribute
{
    public string predicateName;
    

    /// <summary>
    /// The name of a method on this object that returns a bool, and takes no parameters
    /// The method should return true if this field should be hidden in the inspector, otherwise false
    /// </summary>
    public HideIfAttribute(string predicateName)
    {
        this.predicateName = predicateName;
    }
}