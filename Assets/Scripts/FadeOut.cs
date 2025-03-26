using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    public float fadeDuration = 1f;
    private CanvasGroup fadeCanvas; // UI Canvas Group for fading
    private Scene currentScene;
    
    void Start()
    {
        fadeCanvas = GameObject.FindGameObjectWithTag("FadeIn").GetComponent<CanvasGroup>();
        currentScene = SceneManager.GetActiveScene();
        StartCoroutine(FadeScreen(0f));
    }

    void Update()
    {
        if (currentScene != SceneManager.GetActiveScene() && fadeCanvas.alpha > 0f)
        {
            StartCoroutine(SceneTransition());
        }
            
    }

    IEnumerator SceneTransition()
    {
        yield return StartCoroutine(FadeScreen(0f));
            
        currentScene = SceneManager.GetActiveScene();
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