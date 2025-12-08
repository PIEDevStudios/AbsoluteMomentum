using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BubbleProjectile : NetworkBehaviour
{
    public float bubbleTime = 1f, bubbleSpeed = 35f;
    public ulong playerID;
    void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) return;
        
        Player player = collision.gameObject.transform.root.GetComponentInChildren<Player>();
        if (player != null)
        {
            player.StartCoroutine(BubblePlayer(player));
            GetComponent<Collider>().enabled = false;
            StartCoroutine(DespawnAfterSeconds(1f));
        }
    }

    private IEnumerator DespawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkObject.Despawn();
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
