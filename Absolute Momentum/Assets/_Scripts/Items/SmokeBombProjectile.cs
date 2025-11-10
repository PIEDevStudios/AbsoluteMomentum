using Unity.Netcode;
using UnityEngine;

public class SmokeBombProjectile : NetworkBehaviour
{
    public ParticleSystem particles;
    
    void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        if (!collision.gameObject.CompareTag("Player")) {
            PlayParticlesClientRpc();
            NetworkObject.Despawn();
        }
    }

    [ClientRpc]
    void PlayParticlesClientRpc()
    {
        particles.Play();
    }
    
    
    
    
}
