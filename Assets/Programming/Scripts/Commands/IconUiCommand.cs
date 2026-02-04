using System.Collections;
using UnityEngine;

public class IconUiCommand : Command
{
    public override float cost => 0f;
    public override IEnumerator Execute()
    {
        yield return null;
    }
    public override void Break()
    {

    }
    
}
