using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;

public class PlayerUIManager : NetworkBehaviour
{
    [SerializeField] private GameObject inGameUI, resultsUI, leaderboardParent;
    [SerializeField] private GameObject LeaderBoardUIElement;
    [SerializeField] private Player player;

    public void OnEnable()
    {
        RaceManager.Instance.OnTimeSubmitted += UpdateResultsUI;
    }

    public void OnDisable()
    {
        RaceManager.Instance.OnTimeSubmitted -= UpdateResultsUI;
    }
    
    public void DisplayResultsUI()
    {
        inGameUI.SetActive(false);
        UpdateResultsUI();
        resultsUI.SetActive(true);
    }
    
    public void DisplayInGameUI()
    {
        inGameUI.SetActive(true);
        resultsUI.SetActive(false);
    }

    public void HideAllUI()
    {
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
            
            GameObject element = Instantiate(LeaderBoardUIElement, leaderboardParent.transform);
            PlayerLeaderboardElement elementScript = element.GetComponent<PlayerLeaderboardElement>();
            elementScript.positionText.text = (i+1).ToString();
            elementScript.timeText.text = raceTimes[i].Value.ToString("0:00.00");
            elementScript.usernameText.text = "Player " + raceTimes[i].Key;
        }
        
    }
    
    
}
