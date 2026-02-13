using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuBehavior : MonoBehaviour
{
    public Animator ani;
    public string SceneName;
    void Update()
    {
        if(Input.anyKey && !ani.GetBool("isPressingAnyButton"))
        {
            ani.SetBool("isPressingAnyButton", true);
        }
    }
    public void Continue()
    {
        SceneManager.LoadScene(SceneName);
    }
    public void NewGame()
    {
        SceneManager.LoadScene(SceneName);
    }
    public void Load()
    {
        SceneManager.LoadScene(SceneName);
    }
    public void Settings()
    {
    }
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
