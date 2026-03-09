using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : Service<SceneChanger>
{
    private void Awake()
    {
        Register();
    }

    public void GoToScene(string nextSceneName)
    {
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
