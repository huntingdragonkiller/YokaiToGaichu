using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public string sceneName;
    public string testScene;
    
    public void GameStart()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(sceneName);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
