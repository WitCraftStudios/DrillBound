using UnityEngine;
using System.Collections;

public class WarningPanelFlasher : MonoBehaviour
{
    public float flashInterval = 0.5f; // Time in seconds for each fade in/out
    public float fadeSpeed = 3f; // How fast to fade
    private CanvasGroup canvasGroup;
    private bool fadingIn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Use CanvasGroup for smooth alpha, or just enable/disable the GameObject
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        StartCoroutine(FlashRoutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    System.Collections.IEnumerator FlashRoutine()
    {
        while (true)
        {
            // Fade in
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime * fadeSpeed;
                if (canvasGroup.alpha > 1f) canvasGroup.alpha = 1f;
                yield return null;
            }
            yield return new WaitForSeconds(flashInterval);
            // Fade out
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                if (canvasGroup.alpha < 0f) canvasGroup.alpha = 0f;
                yield return null;
            }
            yield return new WaitForSeconds(flashInterval);
        }
    }
}
