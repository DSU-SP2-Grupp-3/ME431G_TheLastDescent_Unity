using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class KeyHooker : MonoBehaviour
{
    [SerializeField]
    private KeyCodeHook[] hooks;

    private void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
        {
            OnKeyPressed(Event.current.keyCode);
        }
    }

    private void OnKeyPressed(KeyCode keyCode)
    {
        IEnumerable<KeyCodeHook> matching = hooks.Where(h => h.keyCode == keyCode);

        foreach (KeyCodeHook hook in matching)
        {
            hook.OnKeyPressed?.Invoke();
        }
    }

    [Serializable]
    private class KeyCodeHook
    {
        public KeyCode keyCode;
        public UnityEvent OnKeyPressed;
    }
}