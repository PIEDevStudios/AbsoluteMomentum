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
    [SerializeField] private OwnerNetworkAnimator ownerNetworkAnimator;

    void Awake()
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
        ownerNetworkAnimator.enabled = false;
        Destroy(player.graphics.gameObject);
        player.graphics = Instantiate(data.CharacterPrefab, transform).transform;
        player.animator = player.graphics.GetComponentInChildren<Animator>();
        ownerNetworkAnimator.Animator = player.animator;
        ownerNetworkAnimator.enabled = true;
    }
}
