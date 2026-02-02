using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InteractionSystem/TestAction")]
public class ActionScriptableObject : InterAct
{

    public FloatRefrence Cost;
    //m- Ignore runtime for now.
    public override FloatRefrence runTime { get => runTime; set => runTime = value; }
    public List<InterAct> interactions;
    //m- Kommer utvecklas vidare. detta är en temp function som inte kommer ha en return type.
    public override void RunAction(float temp)
    {
        if (Cost > temp) return;

        temp =- Cost;

        //m- Datan som behövs refereras borde alltid vara temp, samt att temp blir en datatyp istället för float.
        //m- data bör inte returneras ur denna function, utan actions bör modifiera/använda existerande data i temp.

        foreach (InterAct inter in interactions)
        {
            inter.RunAction(temp);
        }

        return;
    }
}
