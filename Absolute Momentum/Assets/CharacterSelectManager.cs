using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class CharacterSelectManager : MonoBehaviour {
    public CharacterInfo infoPanel;
    public CharacterData[] characters;
    public List<TriangleButton> characterButtons;
    
    // Store original positions for each button
    private Dictionary<TriangleButton, Vector3> originalPositions;

    public void Start() {
        // Initialize the dictionary to store original positions
        originalPositions = new Dictionary<TriangleButton, Vector3>();
        
        for (int i = 0; i < characters.Length; i++) {
            characterButtons[i].Initialize(characters[i], i, this);
            // Store each button's original position
            originalPositions[characterButtons[i]] = characterButtons[i].transform.localPosition;
        }
    }

    public void OnCharacterSelected(int index) {
        infoPanel.UpdateInfo(characters[index]);
    }

    public void HideOtherButtons(TriangleButton except) {
        foreach (var btn in characterButtons) {
            if (btn != except) {
                btn.gameObject.transform.DOLocalMoveX(2000f, 0.25f).SetEase(Ease.InBack); 
            }
        }
    }

    public void ShowAllButtons() {
        foreach (var btn in characterButtons) {
            // Move each button back to its original position
            if (originalPositions.ContainsKey(btn)) {
                btn.gameObject.transform.DOLocalMove(originalPositions[btn], 0.25f).SetEase(Ease.OutBack);
            }
        }

        infoPanel.HidePanel();
    }
}