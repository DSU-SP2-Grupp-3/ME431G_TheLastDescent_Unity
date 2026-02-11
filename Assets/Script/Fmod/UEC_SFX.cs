using System;
using System.Linq;
using FMOD.Studio;
using UnityEngine;

public class UEC_SFX : MonoBehaviour
{
    #region Definitioner
    
    //Enumerables
    public enum FindMethodType
    {
        JustGameObjects,
        ObjectsWithTags
    }

    public enum ActionTypeSelect
    {
        Play,
        Stop,
        SetParameter
    }
    
    //Strings
    public string[] objectTags;
    
    //Structs
    [System.Serializable]
    public struct EventParameter
    {
        public string name;
        public float value;
    }
    
    [System.Serializable]
    public struct Command
    {
        public ActionTypeSelect actionType;
        public EventParameter[] eventParameters;
        public bool allowFadeout;
        
    }

    #endregion
   
    #region Deklarationer
    
    //Enum
    public FindMethodType findMethod;
    
    //Struct
    public Command[] commands;
    
    //Boolean
    
    //Misc
    public GameObject[] uecObjects;
    private UniversalEventController[] uecScripts;
    
    #endregion


    public void InitiateCommand(int index)
    {
        if (commands[index].actionType == ActionTypeSelect.Play)
        {
            PlayEvent();
        }
        else if (commands[index].actionType == ActionTypeSelect.Stop)
        {
            StopEvent();
        }
        else if (commands[index].actionType == ActionTypeSelect.SetParameter)
        {
            SetParameter();
        }
    }

    private void JustObjects()
    {
        uecScripts = new UniversalEventController[uecObjects.Length];
        for (int i = 0; i < uecObjects.Length; i++)
        {
            uecScripts[i] = (uecObjects[i].GetComponent<UniversalEventController>());
        }
    }

    private void FindObjectsWithTags()
    {
        uecScripts = new UniversalEventController[uecObjects.Length];
        foreach (var tag in objectTags)
        {
            uecObjects = GameObject.FindGameObjectsWithTag(tag);

            for (int i = 0; i < uecObjects.Length; i++)
            {
                uecScripts[i] = (uecObjects[i].GetComponent<UniversalEventController>());
            }
        }
        
    }

    private void PlayEvent()
    {
        foreach (var uec in uecScripts)
        {
            uec.SfxInstance.start();
        }
    }

    private void StopEvent()
    {
        foreach (var uec in uecScripts)
        {
            foreach (var x in commands)
            {
                uec.SfxInstance.stop(x.allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            }
        }
    }

    private void SetParameter()
    {
        foreach (var uec in uecScripts)
        {
            foreach (var x in commands)
            {
                foreach (var par in x.eventParameters)
                {
                    uec.SfxInstance.setParameterByName(par.name, par.value);
                }
            }
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (findMethod == FindMethodType.JustGameObjects)
        {
            JustObjects();
        }
        else if (findMethod == FindMethodType.ObjectsWithTags)
        {
            FindObjectsWithTags();
        }
        
        

    }
}
