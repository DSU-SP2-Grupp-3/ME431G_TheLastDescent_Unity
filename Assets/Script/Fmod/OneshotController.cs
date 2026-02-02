using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class OneshotController : MonoBehaviour
{
    [Serializable]
    public struct OneParams
    {
        public string parName;
        public float parValue;
    }
    
    private enum oneName
    {
        None,
        Tjockis,
        TBALVVF,
        Mama,
        Mamma,
        Mamamia,
        GALLILEO,
        BEELZEBUB,
        Spin,
        HitCount,
        LightningEvent,
    }
    [Header("Oneshot Name")]
    [SerializeField] private oneName oneshotName;
    [Header("Parameters")]
    [SerializeField] private OneParams[] oneParams;
    private int refIndex;

    private GameObject aM;
    private AudioManager audioManager;
    private EventInstance oneInstance;
    
    [Header("Settings")]
    public bool startWithParam = false; //Kör eventet med parametrar.
    public bool playOnStart = false; //Kör eventet då Start tillkallas. 
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aM = GameObject.FindGameObjectWithTag("AudioManager");
        audioManager = aM.GetComponent<AudioManager>();
        var none = false;
        //Kollar om oneRef är tom.
        if (audioManager.oneRef == null || audioManager.oneRef.Length == 0)
        {
            Debug.Log("[" + gameObject.name + "] " + "The Array oneRef is empty or not defined.");
        }
        else
        {
            //Vid fallet då oneName skulle vara satt som "None", så kollas det här.
            switch (oneshotName) 
            {
                case oneName.None:
                    none = true;
                    Debug.Log("[" + gameObject.name + "] " +  "Oneshot event not selected.");
                    break;
            }
            /*Om den lokala variabeln one är false och playOnStart är true,
             så tillåter vi PlayOneShot-metoden att köras.*/
            if (!none && playOnStart)
            {
                PlayOneshot();
            }
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Sätter värdet på refIndex, som bestämmer vilken EventReference ur listan oneRef som tillkallas vid "CreateInstance" i Start.
    private void SetIndex(out bool isNone)
    {
        bool oNIsNone = false;
        
        if (oneshotName == oneName.None)
        {
            oNIsNone = true;
        }
        else
        {
            for (int i = 0; i < audioManager.oneRef.Length; i++)
            {
                string nameString = audioManager.oneRef[i].Path.Remove(0, 16);

                if (nameString == oneshotName.ToString())
                {
                    refIndex = i;
                    Debug.Log("[" + gameObject.name + "] " + "oneshotName index set to " + i);
                }
            }
        }
        isNone = oNIsNone;
    }

    public void PlayOneshot()
    {
        SetIndex(out bool isNone);
        if (isNone)
        {
            Debug.Log("[" + gameObject.name + "] " + "Can't play Oneshot event, because oneshotName index is not set.");
        }
        else if (!isNone)
        {
            oneInstance = RuntimeManager.CreateInstance(audioManager.oneRef[refIndex]);
            if (startWithParam)
            {
                SetEventParams();
            }
        
            oneInstance.start();
            oneInstance.release();
            Debug.Log("[" + gameObject.name + "] " +  "Playing Oneshot event.");    
        }
        
    }

    //Sätter enskilt parametervärdet för det aktiva fmod-eventet i oneInstance.
    public void SetOneshotParam(string parName, float parValue)
    {
        oneInstance.setParameterByName(parName, parValue);
    }
    
    //Sätter alla parametrarna i OneParams-arrayen för det aktiva fmod-eventet i oneInstance.
    private void SetEventParams()
    {
        oneInstance.getDescription(out EventDescription desc);
        desc.getParameterDescriptionCount(out int count);
        
        //Kollar om oneParams har OneParams structs i sig
        if (oneParams == null || oneParams.Length == 0)
        {
            Debug.LogError("[" + gameObject.name + "] " + "Parameter array empty or not defined!");
        }
        else
        {   
            //Loopar igenom arrayen 
            foreach (var x in oneParams)
            {
                bool matchfound = false;
                bool valOver = true;
                
                for (int i = 0; i < count; i++)
                {
                    //Kollar om parametervärdena name och value överensstämmer med det aktiva eventet.
                    desc.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION paraDesc);
                    if (paraDesc.name == x.parName)
                    {
                        matchfound = true;
                    }

                    if (x.parValue <= paraDesc.maximum)
                    {
                        valOver = false;
                    }
                    
                }
                
                if (matchfound && valOver == false)
                {
                    //Vid korrekt namngiven parameter och värdet är inom parameterns maximala värde.
                    audioManager.muInstance.setParameterByName(x.parName, x.parValue);
                    Debug.Log("[" + gameObject.name + "] " + x.parName + " is set to " + x.parValue);
                }
                else if (matchfound && valOver) 
                {
                    //Om parametervärdet överskrids
                    Debug.LogError("[" + gameObject.name + "] " + x.parName+ " found, but its maximum value was outside of allowed range.");
                }
                else
                {
                    //Om parameternamnet ej hittas i eventet
                    Debug.LogError("[" + gameObject.name + "] " + "Could not set parameter!" + x.parName + " not found.");
                    
                }
                
                
            }
            
        }
        
        
    }
}
