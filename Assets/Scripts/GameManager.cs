using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public GameObject winMenu;
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
        if (Input.GetKeyDown(KeyCode.Escape) && !deathMenu.activeSelf && !winMenu.activeSelf)
        {
            if(!isPaused)
                Pause();
            else
                Unpause();
        }

        if (isPaused == false && !deathMenu.activeSelf && !winMenu.activeSelf)
        {
            Time.timeScale = 1f;
        }

        if (isPaused && !deathMenu.activeSelf && !winMenu.activeSelf)
        {
            Time.timeScale = 0f;
        }
    }

    public void Unpause()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        deathMenu.SetActive(false);
        player.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        player.GetComponent<PlayerStats>().ResetStats();
        isPaused = false;
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void PlayerDeath()
    {
        isPaused = true;
        player.GetComponent<PlayerCheckpoint>().Respawn();
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
    
    IEnumerator WinTransition()
    {
        yield return StartCoroutine(FadeScreen(1f));
        Time.timeScale = 0;
    }
    
    public IEnumerator FadeScreen(float targetAlpha)
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

    public void WinGame()
    {
        isPaused = true;
        winMenu.SetActive(true);
        fadeCanvas = GameObject.FindGameObjectWithTag("WinMenu").GetComponent<CanvasGroup>();
        StartCoroutine(WinTransition());
    }
}
