using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class CheckpointTrigger : MonoBehaviour 
{
    [SerializeField] private int checkpointID;
    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) { 
            NetworkObject networkObject = other.transform.root.GetComponent<NetworkObject>();
            ulong playerID = networkObject.OwnerClientId;
            RaceManager.Instance.UpdateCheckpoint(playerID, checkpointID);
        }
    }
}
