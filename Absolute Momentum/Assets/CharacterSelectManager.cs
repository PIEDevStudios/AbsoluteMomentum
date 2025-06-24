using UnityEngine;

public class CharacterSelectManager : MonoBehaviour {
    public CharacterInfo infoPanel;
    public CharacterData[] characters;

    public void OnCharacterSelected(int index) {
        infoPanel.UpdateInfo(characters[index]);
    }
}
