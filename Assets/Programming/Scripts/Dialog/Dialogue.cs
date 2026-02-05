using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public string name;
    [TextArea]
    public string[] sentences;
}