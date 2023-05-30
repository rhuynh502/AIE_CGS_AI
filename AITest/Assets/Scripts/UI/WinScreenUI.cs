using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenUI : MonoBehaviour
{
    public void OnQuitToMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void OnQuitToDesktop()
    {
        Application.Quit();
    }

    public void OnRetry()
    {
        Time.timeScale = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
