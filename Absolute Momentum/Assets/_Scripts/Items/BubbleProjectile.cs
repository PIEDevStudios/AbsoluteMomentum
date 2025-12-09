using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class BubbleProjectile : NetworkBehaviour
{
    public float bubbleTime = 1f, bubbleSpeed = 35f;
    public ulong ItemUserClientID;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Invoke(nameof(DespawnBubbleServerRpc), 5f);
    }
    
    void OnTriggerEnter(Collider collision)
    {
        
        if (!IsServer) return;
        
        Debug.Log("Bubble hit: " + collision.gameObject.name);
        
        Player player = collision.gameObject.transform.root.GetComponentInChildren<Player>();
        
        if (player != null && player.OwnerClientId != ItemUserClientID)
        {
            BubblePlayerClientRpc(player.NetworkObjectId);
            GetComponent<Collider>().enabled = false;
            DespawnBubbleServerRpc();
        }
    }

    
    
    [ServerRpc(RequireOwnership = false)]
    private void DespawnBubbleServerRpc()
    {
        NetworkObject.Despawn(true);
    }

    [ClientRpc]
    private void BubblePlayerClientRpc(ulong networkObjectId)
    {
        Player player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].GetComponentInChildren<Player>();
        player.StartCoroutine(BubblePlayer(player));
    }
    
    private IEnumerator BubblePlayer(Player player)
    {
        Debug.Log("Bubbling Player: " + player.OwnerClientId);
        float timer = 0f;
        player.rb.linearVelocity = Vector3.zero;
        while (timer < bubbleTime)
        {
            player.rb.linearVelocity = new Vector3(0f,player.rb.linearVelocity.y,0f);
            player.rb.AddForce(player.transform.up * bubbleSpeed, ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }
    }
    
    
    
    
}
