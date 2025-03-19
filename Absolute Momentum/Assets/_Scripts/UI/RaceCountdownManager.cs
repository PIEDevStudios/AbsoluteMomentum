using System.Collections;
using UnityEngine;
using TMPro;
using System; 

public class RaceCountdownManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText; // Reference to TextMeshProUGUI
    public event Action OnCountdownFinished; // Event triggered when countdown finishes

    private void Start()
    {
        countdownText.gameObject.SetActive(false);
    }
    
    public void StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        countdownText.gameObject.SetActive(true); // Show the text

        int countdown = 3;
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false); // Hide after countdown

        OnCountdownFinished?.Invoke(); // Invoke the event
    }
}
