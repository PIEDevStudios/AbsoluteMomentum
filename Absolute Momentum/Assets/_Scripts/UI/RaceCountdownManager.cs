using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class RaceCountdownManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText; // Reference to TextMeshProUGUI
    public event Action OnCountdownFinished; // Event triggered when countdown finishes

    [SerializeField] private Player[] players; // Reference to all players

    private void Start()
    {
        countdownText.gameObject.SetActive(false);
    }

    public void StartCountdown()
    {
        // Disable player input before countdown starts
        DisablePlayerInput();

        countdownText.gameObject.SetActive(true);
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
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

        // Enable player input after countdown finishes
        EnablePlayerInput();

        OnCountdownFinished?.Invoke(); // Invoke the event
    }

    // Method to disable player input
    private void DisablePlayerInput()
    {
        foreach (var player in players)
        {
            player.GetComponent<PlayerInput>().enabled = false; // Disable input on each player
        }
    }

    // Method to enable player input after the countdown finishes
    private void EnablePlayerInput()
    {
        foreach (var player in players)
        {
            player.GetComponent<PlayerInput>().enabled = true; // Enable input on each player
        }
    }
}
