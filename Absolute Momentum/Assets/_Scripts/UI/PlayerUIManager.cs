using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using TMPro;

public class PlayerUIManager : NetworkBehaviour
{
    [SerializeField] private GameObject inGameUI, resultsUI, levelPreviewUI, leaderboardParent;
    [SerializeField] private GameObject LeaderBoardUIElement;
    [SerializeField] private TextMeshProUGUI timerText;
    [field:SerializeField] public RaceCountdownManager raceCountdownManager {get; private set;}
    [field:SerializeField] public DeathScreenManager deathScreenManager { get; private set; }
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI lapText;

    public void OnEnable()
    {
        RaceManager.Instance.OnTimeSubmitted += UpdateResultsUI;
    }

    public void OnDisable()
    {
        RaceManager.Instance.OnTimeSubmitted -= UpdateResultsUI;
    }


    private void Update()
    {
        if (!player.IsOwner) return;
        float time = player.playerRaceTimeManager.timer;
        int minutes = Mathf.FloorToInt(time / 60);
        time -= minutes * 60;
        timerText.text = minutes + ":" + time.ToString("00.00");

        if (Input.GetKeyDown(KeyCode.J))
        {
            deathScreenManager.PlayDeathScreen(0);
        }

        lapText.text = "Lap " + (RaceManager.Instance.GetPlayerLaps(player.OwnerClientId)+1).ToString();

        
        
    }
    
    public void DisplayResultsUI()
    {
        HideAllUI();
        UpdateResultsUI();
        Cursor.lockState = CursorLockMode.Confined;
        resultsUI.SetActive(true);
    }
    
    public void DisplayInGameUI()
    {
        HideAllUI();
        inGameUI.SetActive(true);
    }

    public void DisplayLevelPreviewUI()
    {
        HideAllUI();
        levelPreviewUI.SetActive(true);
    }

    public void HideAllUI()
    {
        Cursor.lockState = CursorLockMode.Locked;
        levelPreviewUI.SetActive(false);
        inGameUI.SetActive(false);
        resultsUI.SetActive(false);
    }

    public void UpdateResultsUI()
    {
        Debug.Log("Update UI");
        
        Utilites.DestroyAllChildren(leaderboardParent);
        
        var raceTimes = RaceManager.Instance.getRaceTimes().OrderBy(pair => pair.Value).ToList();

        for(int i = 0; i < raceTimes.Count; i++)
        {
            float time = raceTimes[i].Value;
            int minutes = Mathf.FloorToInt(time / 60);
            time -= minutes * 60;
            GameObject element = Instantiate(LeaderBoardUIElement, leaderboardParent.transform);
            PlayerLeaderboardElement elementScript = element.GetComponent<PlayerLeaderboardElement>();
            elementScript.positionText.text = (i+1).ToString();
            elementScript.timeText.text = minutes + ":" + time.ToString("00.00");
            elementScript.usernameText.text = "Player " + raceTimes[i].Key;

            if (raceTimes[i].Key == NetworkManager.LocalClientId)
            {
                elementScript.timeText.color = Color.red;
                elementScript.usernameText.color = Color.red;
                elementScript.positionText.color = Color.red;
            }
            else
            {
                elementScript.timeText.color = Color.white;
                elementScript.usernameText.color = Color.white;
                elementScript.positionText.color = Color.white;
            }
            
        }
        
    }
    
    
}
