using UnityEngine;
using System.Collections;

public class AutoFadeHorns : MonoBehaviour
{
    public float fadeDuration = 6f;
    
    void Start()
    {
        StartCoroutine(FadeAndDestroy());
    }
    
    IEnumerator FadeAndDestroy()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        Destroy(gameObject); // Удаляем объект
    }
}
