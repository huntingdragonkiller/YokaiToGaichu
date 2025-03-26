using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public  bool isPaused;
    public GameObject player;
    
    public float fadeDuration = 1f;
    private CanvasGroup fadeCanvas; // UI Canvas Group for fading

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused)
                Pause();
            else
                Unpause();
        }
    }

    public void Unpause()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        deathMenu.SetActive(false);
        player.SetActive(true);
        player.GetComponent<PlayerCheckpoint>().Respawn();
        player.GetComponent<PlayerStats>().ResetStats();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayerDeath()
    {
        player.SetActive(false);
        deathMenu.SetActive(true);
        fadeCanvas = GameObject.FindGameObjectWithTag("DeathMenu").GetComponent<CanvasGroup>();
        StartCoroutine(DeathTransition());
    }
    
    IEnumerator DeathTransition()
    {
        yield return StartCoroutine(FadeScreen(1f));
        Time.timeScale = 0;
    }
    
    IEnumerator FadeScreen(float targetAlpha)
    {
        float startAlpha = fadeCanvas.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }
}
