using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public string nextSceneName; // Name of the next scene to load
    public float fadeDuration = 1f;
    private CanvasGroup _fadeCanvas; // UI Canvas Group for fading
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _fadeCanvas = GameObject.FindGameObjectWithTag("FadeIn").GetComponent<CanvasGroup>();
            Debug.Log(_fadeCanvas);
            StartCoroutine(TransitionToNextLevel());
        }
    }

    void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name not set!");
        }
    }

    void WinGame()
    {
        Debug.Log("Game Won!");
        Application.Quit();
        // Add win logic here, such as showing a UI screen or triggering an event
    }
    
    IEnumerator TransitionToNextLevel()
    {
        // Start fade to black
        yield return StartCoroutine(FadeScreen(1f));
        
        // Load next level or trigger win
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            LoadNextLevel();
        }
        else
        {
            WinGame();
        }
    }
    
    IEnumerator FadeScreen(float targetAlpha)
    {
        float startAlpha = _fadeCanvas.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            _fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _fadeCanvas.alpha = targetAlpha;
    }
}
