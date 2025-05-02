using Unity;
using UnityEngine;
using System;

public class CharacterSelectionTrigger : MonoBehaviour
{
    public enum Character{
        Baku,
        Sangeo,
        Bianca,
        Etoile,
        LeiLei
    }
    [SerializeField] private Character character;

    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            ChangeCharacter(character);
        }
    }

    private void ChangeCharacter(Character selectedCharacter) {
        if (selectedCharacter == Character.Baku) {
            Debug.Log("Character switching to Baku");
        } else if (selectedCharacter == Character.Sangeo) {
            Debug.Log("Character switching to Sangeo");
        } else if (selectedCharacter == Character.Bianca) {
            Debug.Log("Character switching to Bianca");
        } else if (selectedCharacter == Character.Etoile) {
            Debug.Log("Character switching to Etoile");
        } else if (selectedCharacter == Character.LeiLei) {
            Debug.Log("Character switching to LeiLei");
        } else {
            Debug.Log("Invalid character selection");
        }
    }
}