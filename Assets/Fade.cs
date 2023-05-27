using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public static Action fadeSky;

    SpriteRenderer spriteRenderer;
    [SerializeField]
    float fadeDuration;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        fadeSky += FadeBackground;
    }

    private void FadeBackground()
    {
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        // Ensure the final color is set to the target value
        spriteRenderer.color = endColor;
    }
}
