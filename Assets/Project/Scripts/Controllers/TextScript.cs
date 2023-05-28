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
    string[] radioTextArray0;
    [SerializeField]
    string[] radioTextArray1;
    [SerializeField]
    string[] radioTextArray2;
    [SerializeField]
    string[] radioTextArray3;
    [SerializeField]
    string[] radioTextArray4;
    [SerializeField]
    string[] radioTextArray5;
    [SerializeField]
    string[] radioTextArray6;
    [SerializeField]
    string[] radioTextArray7;
    [SerializeField]
    string[] radioTextArray8;

    List<string[]> allTexts; 

    [SerializeField]
    float fadeDuration;

    [SerializeField]
    float displayDuration;

    [SerializeField]
    Image panel;

    public static Action<int> displayRadioText;
    public static bool textIsBeingDisplayed;

    private void Awake()
    {
        allTexts = new List<string[]>();
        allTexts.Add(radioTextArray0);
        allTexts.Add(radioTextArray1);
        allTexts.Add(radioTextArray2);
        allTexts.Add(radioTextArray3);
        allTexts.Add(radioTextArray4);
        allTexts.Add(radioTextArray5);
        allTexts.Add(radioTextArray6);
        allTexts.Add(radioTextArray7);

        displayRadioText += DisplayAndFade;
        panel = GetComponent<Image>();

        
    }


    private void DisplayAndFade(int index)
    {
        StartCoroutine(FadeOutAndIn(index, displayDuration));
    }

    IEnumerator FadeOutAndIn(int index, float displayDuration)
    {
        textIsBeingDisplayed = true;
        foreach (var text in allTexts[index])
        {
            radioText.text = text;
            yield return FadeIn();
            yield return new WaitForSeconds(displayDuration);
            yield return FadeOut();
            yield return new WaitForSeconds(fadeDuration);
        }
        textIsBeingDisplayed = false;
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