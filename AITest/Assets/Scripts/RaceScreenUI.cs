using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceScreenUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI countdownText;
    private float countDown = 3;

    private void Awake()
    {
        Time.timeScale = 0;
    }
    private void Update()
    {
        if(countdownText.isActiveAndEnabled)
        {
            countdownText.text = countDown.ToString();
            countDown -= Time.unscaledDeltaTime;
        }
    }
    public void StartRace()
    {
        StartCoroutine(StartRace_CR());
    }

    private IEnumerator StartRace_CR()
    {
        countdownText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(3);

        button.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1;

    }
}
