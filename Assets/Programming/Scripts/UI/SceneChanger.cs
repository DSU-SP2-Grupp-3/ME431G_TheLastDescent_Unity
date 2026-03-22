using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : Service<SceneChanger>
{
    public event Action<string> OnGoToScene;
    
    private void Awake()
    {
        Register();
    }

    public void GoToScene(string nextSceneName)
    {
        Register();
        OnGoToScene?.Invoke(nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }

    public void CloseApplication()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
