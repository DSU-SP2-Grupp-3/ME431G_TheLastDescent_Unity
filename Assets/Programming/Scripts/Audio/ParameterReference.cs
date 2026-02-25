using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public static class ParameterReference
{
    public static List<PARAMETER_DESCRIPTION> GetParameters(EventReference eventReference)
    {
        List<PARAMETER_DESCRIPTION> paramDesc = new();
        RuntimeManager.StudioSystem.getEventByID(eventReference.Guid, out EventDescription desc);
        desc.getParameterDescriptionCount(out int count);
        for(int i = 0; i < count; i++)
        {
            desc.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION param);
                    Debug.Log($"Parameter: {param.name}");
            paramDesc.Add(param);
        }
        return paramDesc;
    }
}
