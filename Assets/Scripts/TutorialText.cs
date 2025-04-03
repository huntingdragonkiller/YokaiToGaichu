using System.Collections;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    [Header("Tutorial Text")]
    public GameObject text;
    
    [Header("Text fade settings")]
    public float fadeDuration = 0.2f;
    public CanvasGroup fadeCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            FadeIn();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            StartCoroutine(FadeOut());
    }
    
    void FadeIn()
    {
        text.SetActive(true);
        StartCoroutine(FadeText(1f));
    }
    
    IEnumerator FadeOut()
    {
        yield return StartCoroutine(FadeText(0f));
        text.SetActive(false);
    }
    
    IEnumerator FadeText(float targetAlpha)
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
