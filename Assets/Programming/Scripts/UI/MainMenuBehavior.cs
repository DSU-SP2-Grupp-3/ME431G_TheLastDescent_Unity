using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehavior : MonoBehaviour
{
    public Animator ani;
    public string NewSceneName;
    public string ContinueSceneName;

    private void Start()
    {
        ani.SetBool("isPressingAnyButton", false);
    }

    void Update()
    {
        if (Input.anyKey && !ani.GetBool("isPressingAnyButton"))
        {
            ani.SetBool("isPressingAnyButton", true);
        }
    }

    public void Continue()
    {
        SceneManager.LoadScene(ContinueSceneName);
    }
    public void NewGame()
    {
        SceneManager.LoadScene(NewSceneName);
    }
    public void Load()
    {
        SceneManager.LoadScene(ContinueSceneName);
    }
    public void Settings() { }
    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}