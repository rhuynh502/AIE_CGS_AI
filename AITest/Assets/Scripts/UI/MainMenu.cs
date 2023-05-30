using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform mainScreen;
    [SerializeField] private Transform allButtons;
    [SerializeField] private Transform buttonSelect;
    [SerializeField] private Transform buttonStages;
    [SerializeField] private Transform stage1;
    [SerializeField] private Transform stage2;
    [SerializeField] private Transform stage3;

    private CanvasGroup mainScreenCanvasGroup;
    private CanvasGroup allButtonsCanvasGroup;

    private void Start()
    {
        Time.timeScale = 1;

        mainScreenCanvasGroup = mainScreen.GetComponent<CanvasGroup>();
        allButtonsCanvasGroup = allButtons.GetComponent<CanvasGroup>();
    }

    public void Continue()
    {
        StartCoroutine(FadeMainMenu_CR());
    }

    public void StageSelect()
    {
        StartCoroutine(FadeOutButtonSelect_CR());
    }

    public void BackToMenu()
    {
        StartCoroutine(FadeOutButtonStages_CR());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenStagePreview(int stage)
    {
        ResetStagePreview();
        if (stage == 1)
            stage1.gameObject.SetActive(true);
        else if (stage == 2)
            stage2.gameObject.SetActive(true);
        else if (stage == 3)
            stage3.gameObject.SetActive(true);
    }

    public void StartStage(int stage)
    {
        if (stage == 1)
            SceneManager.LoadScene("Map1");
        else if (stage == 2)
            SceneManager.LoadScene("Map2");
        else if (stage == 3)
            SceneManager.LoadScene("Map3");
    }

    private void ResetStagePreview()
    {
        stage1.gameObject.SetActive(false);
        stage2.gameObject.SetActive(false);
        stage3.gameObject.SetActive(false);
    }

    private IEnumerator FadeMainMenu_CR()
    {
        float startingTime = 0;

        while(mainScreenCanvasGroup.alpha > 0)
        {
            mainScreenCanvasGroup.alpha = Mathf.Lerp(1, 0, Mathf.Clamp01(startingTime));
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
            allButtonsCanvasGroup.alpha = Mathf.Lerp(0, 1, startingTime);
            startingTime += Time.deltaTime;

            yield return null;
        }

        allButtonsCanvasGroup.interactable = true;
    }

    private IEnumerator FadeOutButtonSelect_CR()
    {
        float startingTime = 0;

        while (allButtonsCanvasGroup.alpha < 1)
        {
            allButtonsCanvasGroup.alpha = Mathf.Lerp(1, 0, startingTime / 2);
            startingTime += Time.deltaTime;

            yield return null;
        }
        buttonSelect.gameObject.SetActive(false);
        buttonStages.gameObject.SetActive(true);
        StartCoroutine(FadeInButtons_CR());

    }


    private IEnumerator FadeOutButtonStages_CR()
    {
        float startingTime = 0;

        while (allButtonsCanvasGroup.alpha < 1)
        {
            allButtonsCanvasGroup.alpha = Mathf.Lerp(1, 0, startingTime / 2);
            startingTime += Time.deltaTime;

            yield return null;
        }
        ResetStagePreview();

        buttonStages.gameObject.SetActive(false);
        buttonSelect.gameObject.SetActive(true);

        StartCoroutine(FadeInButtons_CR());

    }
}
