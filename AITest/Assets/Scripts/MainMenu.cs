using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform mainScreen;
    [SerializeField] private Transform allButtons;

    private CanvasGroup mainScreenCanvasGroup;
    private CanvasGroup allButtonsCanvasGroup;

    private void Awake()
    {
        mainScreenCanvasGroup = mainScreen.GetComponent<CanvasGroup>();
        allButtonsCanvasGroup = allButtons.GetComponent<CanvasGroup>();
    }

    public void Continue()
    {
        StartCoroutine(FadeMainMenu_CR());
    }

    private IEnumerator FadeMainMenu_CR()
    {
        float startingTime = 0;

        while(mainScreenCanvasGroup.alpha > 0)
        {
            mainScreenCanvasGroup.alpha = Mathf.Lerp(1, 0, Mathf.Clamp01(startingTime / 2));
            startingTime += Time.deltaTime;

            yield return null;
        }

        StartCoroutine(FadeInButtons_CR());
        mainScreenCanvasGroup.interactable = false;
    }

    private IEnumerator FadeInButtons_CR()
    {
        float startingTime = 0;

        while(allButtonsCanvasGroup.alpha < 1)
        {
            allButtonsCanvasGroup.alpha = Mathf.Lerp(0, 1, startingTime / 2);
            startingTime += Time.deltaTime;

            yield return null;
        }

        allButtonsCanvasGroup.interactable = true;
    }
}
