using System;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NameTagGetter : NetworkBehaviour
{
    [SerializeField] private TextMeshPro nameTagText; // Assign this in the Inspector
    private NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            
            // Disable this clients own name tag
            nameTagText.enabled = false;
            
            // Set this player's username
            playerName.Value = "Player " + OwnerClientId;
        }

        // Update nametag when this object spawns
        UpdateNameTag();
        
        
        playerName.OnValueChanged += (FixedString64Bytes oldValue, FixedString64Bytes newValue) =>
        {
            UpdateNameTag();
        };
    }
    private void UpdateNameTag()
    {
        nameTagText.text = playerName.Value.ToString();
    }
    
    
}