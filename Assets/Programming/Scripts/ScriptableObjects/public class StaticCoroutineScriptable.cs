using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class StaticCoroutineScriptable : ScriptableObject
{
    //-Ma. Watch this shit.
    private static StaticCoroutineScriptable instance = null;
    public static StaticCoroutineScriptable Instance()
    {
        if (instance == null) instance = new StaticCoroutineScriptable();
        return instance;
    }
    public IEnumerator LookAt(WorldAgent A, WorldAgent B, float t)
    {
        while (t > 0)
        {
            t -= Time.deltaTime;
            A.transform.rotation.SetFromToRotation(A.transform.rotation.eulerAngles - (B.transform.rotation.eulerAngles * t), (A.transform.rotation.eulerAngles * t) - B.transform.rotation.eulerAngles);
            yield return null;
        }


    }
    //-Ma. fyi, this should prob not remain at the end of the project, but will work as a stand in for now.
}
