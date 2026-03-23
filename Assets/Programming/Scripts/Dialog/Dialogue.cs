using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    public string name;
    public Sprite portrait;
    public GameObject Popup;
    public EventScriptable sfx;
    [TextArea]
    public string[] sentences;

}