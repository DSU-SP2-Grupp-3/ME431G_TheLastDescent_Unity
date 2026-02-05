using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog")]
public class DialogueScriptable : ScriptableObject
{
    //-Ma. I need to see how the command is supposed to work fully before I start intergrating them.
    //-Ma. Hoping this is soon.

    public Dialogue[] dialogues;

    //-Ma. Temp function.
    public Dialogue[] GetDialogues()
    {
        return dialogues;
    }
}
