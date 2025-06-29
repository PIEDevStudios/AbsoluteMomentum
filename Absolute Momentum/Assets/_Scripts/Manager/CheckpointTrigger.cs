using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;

public class CheckpointTrigger : NetworkBehaviour 
{
    [SerializeField] private int checkpointID;
    [SerializeField] private Transform respawnPoint;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) { 
            NetworkObject networkObject = other.transform.root.GetComponent<NetworkObject>();
            Player player = networkObject.GetComponentInChildren<Player>();
            
            ulong playerID = networkObject.OwnerClientId;
            RaceManager.Instance.UpdateCheckpoint(playerID, checkpointID, player, respawnPoint);
        }
    }
}
