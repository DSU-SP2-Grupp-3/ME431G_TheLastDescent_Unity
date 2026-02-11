using System.Linq;
using UnityEngine;

public class UEC_SFX : MonoBehaviour
{
    #region Definitioner
    
    //Enumerables
    public enum FindMethodType
    {
        GameObject,
        ObjectWithTag,
        ObjectsWithTag
    }

    public enum ActionTypeSelect
    {
        Play,
        Stop,
        SetParameter
    }
    
    //Strings
    public string objectTag;
    public string[] objectTags;
    
    //Structs
    [System.Serializable]
    public struct EventParameter
    {
        public string name;
        public string value;
    }

    #endregion
   
    #region Deklarationer
    
    //Enum
    public ActionTypeSelect action;
    public FindMethodType findMethod;
    
    //Boolean
    public bool startWithParameter;
    
    
    //Misc
    [SerializeField] private EventParameter[] parameters;
    public GameObject uecObject;
    public GameObject[] uecObjects;
    private UniversalEventController uecScript;
    private UniversalEventController[] uecScripts;
    
    #endregion


    public void Initiate()
    {
        
    }
    private void OnDragDrop()
    {
        uecScript = uecObject.GetComponent<UniversalEventController>(); 
    }
    private void FindObjectWithTag()
    {
        uecObjects = GameObject.FindGameObjectsWithTag(objectTag);
        uecScript = uecObject.GetComponent<UniversalEventController>(); 
    }

    private void FindObjectsWithTags()
    {

        foreach (var tag in objectTags)
        {
            uecObjects = GameObject.FindGameObjectsWithTag(tag);

            foreach (var x in uecObjects)
            {
                uecScripts.Append(x.GetComponent<UniversalEventController>());
            }
        }
        
    }

    private void PlayEvent()
    {
        
    }

    private void StopEvent()
    {
        
    }

    private void SetParameter()
    {
        
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
