using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class InspectTest : MonoBehaviour
{
    private Bank bank;

    public bool forEnum;
    [SerializeField] private bool forTest;

    public enum DropDown1 
    {
        
        Alt1,
        Alt2,
        Alt3,
        
    }
    
    public enum DropDown2
    {
        Alt1,
        Alt2,
        Alt3,
        
    }
    
    public enum DropDown3
    {
        Alt1,
        Alt2,
        Alt3,
        
    }
    
    [SerializeField] DropDown1 dropDown1;
    [SerializeField] DropDown2 dropDown2;
    [SerializeField] DropDown3 dropDown3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RuntimeManager.StudioSystem.getBank("bank:/Master", out Bank bonk);
        bank = bonk;
        
        bank.getEventList(out EventDescription[] events);

        foreach (var i in events)
        {
            i.getPath(out string path);
            Debug.Log(path);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
