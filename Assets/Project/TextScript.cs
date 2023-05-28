using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class TextScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI radioText;

    [SerializeField]
    string[] radioTextArray;

    [SerializeField]
    float fadeDuration;

    [SerializeField]
    float displayDuration;

    [SerializeField]
    Image panel;

    public static Action<int> displayRadioText;

    private void Start()
    {
        displayRadioText += DisplayAndFade;
        panel = GetComponent<Image>();
    }


    private void DisplayAndFade(int index)
    {
        StartCoroutine(FadeOutAndIn(index, displayDuration));
    }

    IEnumerator FadeOutAndIn(int index, float displayDuration)
    {
        radioText.text = radioTextArray[index];
        yield return FadeIn();
        yield return new WaitForSeconds(displayDuration);
        yield return FadeOut();
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color startTextColor = radioText.color;
        Color endTextColor = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);

        Color startPanelColor = panel.color;
        Color endPanelColor = new Color(startPanelColor.r, startPanelColor.g, startPanelColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            radioText.color = Color.Lerp(startTextColor, endTextColor, t);
            panel.color = Color.Lerp(startPanelColor, endPanelColor, t);

            yield return null;
        }

        radioText.color = endTextColor;
        panel.color = endPanelColor;
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color startTextColor = radioText.color;
        Color endTextColor = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 1f);

        Color startPanelColor = panel.color;
        Color endPanelColor = new Color(startPanelColor.r, startPanelColor.g, startPanelColor.b, .7f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            radioText.color = Color.Lerp(startTextColor, endTextColor, t);
            panel.color = Color.Lerp(startPanelColor, endPanelColor, t);

            yield return null;
        }

        radioText.color = endTextColor;
        panel.color = endPanelColor;
    }
}