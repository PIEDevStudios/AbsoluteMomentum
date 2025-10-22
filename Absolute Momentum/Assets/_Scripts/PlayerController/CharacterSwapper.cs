using System;
using UnityEngine;

public enum Characters
{
    Baku,
    Sangeo
}
public class CharacterSwapper : MonoBehaviour
{
    [Serializable]
    public struct CharacterSwapperData
    {
        public Characters character;
        public GameObject CharacterPrefab;
    }

    [SerializeField] private Player player;
    [SerializeField] private CharacterSwapperData[] characterSwapperData;
    private GameObject currentGraphics;

    void Start()
    {
        SwapCharacters(characterSwapperData[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SwapCharacters(characterSwapperData[1]);
        }
    }

    private void SwapCharacters(CharacterSwapperData data)
    {
        Destroy(currentGraphics);
        currentGraphics = Instantiate(data.CharacterPrefab, transform);
        player.animator = currentGraphics.GetComponentInChildren<Animator>();
    }
}
