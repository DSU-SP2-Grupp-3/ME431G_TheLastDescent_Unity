using UnityEngine;

public class UniversalEventController : MonoBehaviour
{
    #region Definitioner
    //Definitioner
    
    public enum EventTypeSelect
    {
        Music,
        Sfx,
        Ambience
    }

    public enum ActionTypeSelect
    {
        Play,
        Stop,
        SetParameter
    }

    public enum SfxTypeSelect
    {
        Characters,
        Enemy,
        UI,
        Interactable
    }
    
    //Music Events
    public enum MusicEventSelect
    {
        Music1,
        Music2,
        Music3,
    }
    
    //Sfx Events
    public enum CharEventSelect
    {
        Test1,
        Test2,
    }

    public enum EnemyEventSelect
    {
        Test1,
        Test2,
    }

    public enum UIEventSelect
    {
        Test1,
        Test2,
    }

    public enum InterEventSelect
    {
        Test1,
        Test2,
    }
    
    //Ambiance Events

    public enum AmbienceEventSelect
    {
        Ambience1,
        Ambience2,
    }
    
    //Parametrar
    [System.Serializable]
    public struct EventParameter
    {
        public string name;
        public float value;
    }
    
    #endregion
    
    #region Deklarationer
    
    private GameObject aM;
    private AudioManager audioManager;
    
    //Boolean
    public bool playOnStart;
    public bool startWithParameters;
    public bool allowFadeout;
    public bool debugMode;
    
    //Enum
    public EventTypeSelect eventTypeSelect;
    public ActionTypeSelect actionTypeSelect;
    public SfxTypeSelect sfxTypeSelect;
    public MusicEventSelect musicEventSelect;
    public CharEventSelect charEventSelect;
    public EnemyEventSelect enemyEventSelect;
    public UIEventSelect uiEventSelect;
    public InterEventSelect interEventSelect;
    public AmbienceEventSelect ambienceEventSelect;
    
    //Array
    public EventParameter[] parameters;
    
    //Intergers
    private int eventIndex;
    
    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aM = GameObject.FindGameObjectWithTag("AudioManager");
        audioManager = aM.GetComponent<AudioManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initiate()
    {
        
    }

    private int SelectedEventType()
    {
        int eventType = 0;
        switch (eventTypeSelect)
        {
            case EventTypeSelect.Music:
                eventType = 1;
                break;
            case EventTypeSelect.Ambience:
                eventType = 2;
                break;
            case EventTypeSelect.Sfx:
                eventType = 3;
                break;
        }
        return eventType;
    }

    private int SelectedActionType()
    {
        int actionType = 0;
        switch (actionTypeSelect)
        {
            case ActionTypeSelect.Play:
                actionType = 1;
                break;
            case ActionTypeSelect.Stop:
                actionType = 2;
                break;
            case ActionTypeSelect.SetParameter:
                actionType = 3;    
                break;
        }
        return actionType;
    }

    private int SelectedSfxType()
    {
        int sfxType = 0;
        switch (sfxTypeSelect)
        {
            case SfxTypeSelect.Characters:
                sfxType = 1;
                break;
            case SfxTypeSelect.Enemy:
                sfxType = 2;
                break;
            case SfxTypeSelect.UI:
                sfxType = 3;
                break;
            case SfxTypeSelect.Interactable:
                sfxType = 4;
                break;
        }
        return sfxType;
    }

    private void SetEventParameters()
    {
        
    }

    private void SetEventIndex()
    {
        if (SelectedActionType() == 1) //Music
        {
            for (int i = 0; i < audioManager.muRef.Length; i++)
            {
                if (audioManager.muRef[i].Path.Contains(musicEventSelect.ToString()))
                {
                    eventIndex = i;
                    break;
                }
            }
        }
        else if (SelectedActionType() == 2) //Ambiance
        {
            for (int i = 0; i < audioManager.ambRef.Length; i++)
            {
                if (audioManager.ambRef[i].Path.Contains(ambienceEventSelect.ToString()))
                {
                    eventIndex = i;
                    break;
                }
            }
        }
        else if (SelectedActionType() == 3) //Sfx
        {
            if (SelectedSfxType() == 1) //Character sfx
            {
                for (int i = 0; i < audioManager.characterRef.Length; i++)
                {
                    if (audioManager.characterRef[i].Path.Contains(charEventSelect.ToString()))
                    {
                        eventIndex = i;
                    }
                }
            }
            else if (SelectedSfxType() == 2) //Enemy sfx
            {
                for (int i = 0; i < audioManager.enemyRef.Length; i++)
                {
                    if (audioManager.enemyRef[i].Path.Contains(enemyEventSelect.ToString()))
                    {
                        eventIndex = i;
                    }
                }
            }
            else if (SelectedSfxType() == 3) //UI sfx
            {
                for (int i = 0; i < audioManager.UIRef.Length; i++)
                {
                    if (audioManager.UIRef[i].Path.Contains(uiEventSelect.ToString()))
                    {
                        eventIndex = i;
                    }
                }
            }
            else if (SelectedSfxType() == 4) //Interactable sfx
            {
                for (int i = 0; i < audioManager.interactRef.Length; i++)
                {
                    if (audioManager.interactRef[i].Path.Contains(interEventSelect.ToString()))
                    {
                        eventIndex = i;
                    }
                }
            }
        }
    }
}
