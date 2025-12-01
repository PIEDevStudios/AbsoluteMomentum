using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GlueProjectile : NetworkBehaviour
{
    public Transform visuals;
    [SerializeField] private float slowdown = 0.5f;
    void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        if (!collision.gameObject.CompareTag("Player"))
        {
            ShowVisualsClientRpc(transform.position);
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity);
            transform.position = hit.point;
            StartCoroutine(DespawnAfterSeconds(5f));
        }
    }
    
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            player.SpeedMultiplier -= slowdown;
        }
    }
    
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            player.SpeedMultiplier += slowdown;
        }
    }

    private IEnumerator DespawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkObject.Despawn();
    }

    [ClientRpc]
    void ShowVisualsClientRpc(Vector3 position)
    {
        visuals.transform.position = position;
        visuals.gameObject.SetActive(true);
        GetComponent<MeshRenderer>().enabled = false;
    }
    
    
    
    
}