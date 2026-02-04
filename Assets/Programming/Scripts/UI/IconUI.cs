using UnityEngine;
using UnityEngine.EventSystems;

public class IconUI : MonoBehaviour, ISelectHandler, IDeselectHandler  
{
    //-Ma. Behöver se hur commands utförs innan detta implenteras ordentligt.
    //-Ma. Väntar tills Sa har implementrat världs klockan.
    public void OnSelect(BaseEventData eventData)
    {
        //-Ma. Put the ability into Queue
        Debug.Log("sup");
    }
    public void OnDeselect(BaseEventData eventData)
    {
        //-Ma. Executes or removes from queue here.
        Debug.Log("Bye");
    }
}
