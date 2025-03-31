using System.Collections;
using UnityEngine;
using TMPro;
using System; 

public class RaceCountdownManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText; // Reference to TextMeshProUGUI
    [SerializeField] private Player player;
    public event Action OnCountdownFinished; // Event triggered when countdown finishes
    private RaceManager raceManager;
    private float countdownValue;
    private void Start()
    {
        countdownText.enabled = false;
        raceManager = RaceManager.Instance;
    }

    private void Update()
    {
        countdownValue = raceManager.GetCountdownTimer();
        
        if (countdownValue > 0)
        {
            if(player != null)
                player.playerInput.enabled = false;
            countdownText.text = countdownValue.ToString();
            countdownText.enabled = true;
        }
        else if (countdownValue == 0)
        {
            StartCoroutine(CountdownCoroutine());
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        countdownText.text = "GO!";
        
        yield return new WaitForSeconds(1f);

        countdownText.enabled = false; // Hide after countdown

        if(player != null)
            player.playerInput.enabled = true;
        
        OnCountdownFinished?.Invoke(); // Invoke the event
    }
}
