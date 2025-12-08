using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject itemPickupPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnTime = 3f;
    private bool IsSpawned = false;
    private GameObject spawnedItem;
    private float timer;


    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        
        if (IsSpawned && spawnedItem == null)
        {
            IsSpawned = false;
            timer = 0f;
        }
        
        timer += Time.deltaTime;

        if (timer >= spawnTime && !IsSpawned)
        {
            SpawnItem();
        }
        
    }

    void SpawnItem()
    {
        IsSpawned = true;
        spawnedItem = Instantiate(itemPickupPrefab, spawnPoint.position, itemPickupPrefab.transform.rotation);
        spawnedItem.GetComponent<NetworkObject>().Spawn();
    }
}
