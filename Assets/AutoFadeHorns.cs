using UnityEngine;
using System.Collections;

public class AutoFadeHorns : MonoBehaviour
{
    public float totalLifetime = 20f;
    public float fadeDuration = 5f;

    void Start()
    {
        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeAndDestroy()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            yield break;
        }

        Color originalColor = sr.color;

        float visibleTime = Mathf.Max(0f, totalLifetime - fadeDuration);
        if (visibleTime > 0f)
        {
            yield return new WaitForSeconds(visibleTime);
        }

        float elapsed = 0f;
        float safeFadeDuration = Mathf.Max(0.0001f, fadeDuration);
        while (elapsed < safeFadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / safeFadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
