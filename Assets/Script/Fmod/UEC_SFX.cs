using UnityEngine;

public class UEC_SFX : MonoBehaviour
{
    public enum FindMethod
    {
        DragDrop,
        ObjectWithTag,
        ObjectsWithTag
    }

    public enum Action
    {
        Play,
        Stop,
        SetParameter
    }

    public string objectTag;
    
    public struct Parameter
    {
        public string name;
        public string value;
    }
    
    private Parameter[] parameters;

    private void FindObjectWithTag()
    {
        
    }

    private void FindObjectsWithTag()
    {
        
    }
    public GameObject uecObject;
    private UniversalEventController uecScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uecScript = uecObject.GetComponent<UniversalEventController>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
