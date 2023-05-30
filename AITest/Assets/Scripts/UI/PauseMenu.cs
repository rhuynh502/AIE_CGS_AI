using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Transform buttons;
    [SerializeField] private RaceScreenUI raceScreenUI;

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            buttons.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void Resume()
    {
        buttons.gameObject.SetActive(false);
        raceScreenUI.StartRace();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
