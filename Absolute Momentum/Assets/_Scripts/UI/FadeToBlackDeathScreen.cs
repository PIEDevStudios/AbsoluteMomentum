using UnityEngine;
using System.Collections;

public class FadeToBlackDeathScreen : BaseDeathScreen
{
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float duration = 1f;

    protected override void OnPlay()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        fadeGroup.alpha = 0f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Clamp01(1 - t / duration);
            yield return null;
        }

        fadeGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
