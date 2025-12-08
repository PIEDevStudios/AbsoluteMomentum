using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SmokeBombProjectile : NetworkBehaviour
{
    public ParticleSystem particles;
    
    void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        if (!collision.gameObject.CompareTag("Player"))
        {
            PlayParticlesClientRpc(transform.position);
            GetComponent<Collider>().enabled = false;
            StartCoroutine(DespawnAfterSeconds(5f));
        }
    }

    private IEnumerator DespawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkObject.Despawn();
    }

    [ClientRpc]
    void PlayParticlesClientRpc(Vector3 position)
    {
        particles.transform.position = position;
        GetComponent<MeshRenderer>().enabled = false;
        particles.Play();
    }
    
    
    
    
}
