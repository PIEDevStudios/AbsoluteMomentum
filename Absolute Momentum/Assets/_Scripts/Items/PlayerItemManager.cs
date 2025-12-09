using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerItemManager : NetworkBehaviour
{
    private Player player;
    [SerializeField] private BaseItem attackItem, movementItem;
    [SerializeField] private Image attackItemImage, movementItemImage; 
    [SerializeField] private float orbitRadius;
    [SerializeField] private ItemPoolSO attackItemPool, movementItemPool;
    [SerializeField] private Transform itemParent; // The parent transform for the items
    public enum ItemType
    {
        Attack,
        Movement
    }

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Update()
    {
        if (!IsOwner) return;
        
       
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectAttackItem();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectMovementItem();

        if (Input.GetKeyDown(KeyCode.Q))
            UseAttackItem();

        if (Input.GetKeyDown(KeyCode.E))
            UseMovementItem();
        
    }

    public void SelectAttackItem()
    {
        if (attackItem) return;

        int index = Random.Range(0, attackItemPool.items.Length);
        ItemSO item = attackItemPool.items[index];
        attackItemImage.sprite = item.PowerupIcon;
        
        SelectAttackItemServerRpc(index);
    }

    [ServerRpc]
    public void SelectAttackItemServerRpc(int index)
    {
        ItemSO item = attackItemPool.items[index];

        attackItem = Instantiate(item.prefab, transform.position, Quaternion.identity, itemParent).GetComponent<BaseItem>();
        attackItem.Initialize(player, player.OwnerClientId);

        NetworkObject networkObject = attackItem.GetComponent<NetworkObject>();
        networkObject.Spawn(true);
        AssignAttackItemClientRpc(networkObject.NetworkObjectId, networkObject.OwnerClientId);
    }
    
    [ClientRpc]
    private void AssignAttackItemClientRpc(ulong networkObjectId, ulong ownerClientId)
    {
        if (!IsOwner) return; // Only run for the owning client

        NetworkObject spawnedObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        attackItem = spawnedObject.GetComponent<BaseItem>();
        attackItem.Initialize(player, player.OwnerClientId);
    }

    
    public void SelectMovementItem()
    {
        if (movementItem) return;

        int index = Random.Range(0, movementItemPool.items.Length);
        ItemSO item = movementItemPool.items[index];
        
        movementItemImage.sprite = item.PowerupIcon;
        
        SelectMovementItemServerRpc(index);
    }
    
    [ServerRpc]
    public void SelectMovementItemServerRpc(int index)
    {
        ItemSO item = movementItemPool.items[index];

        movementItem = Instantiate(item.prefab, transform.position, Quaternion.identity, itemParent).GetComponent<BaseItem>();
        movementItem.Initialize(player, player.OwnerClientId);

        NetworkObject networkObject = movementItem.GetComponent<NetworkObject>();
        networkObject.Spawn(true);
        AssignMovementItemClientRpc(networkObject.NetworkObjectId, networkObject.OwnerClientId);
    }
    
    [ClientRpc]
    private void AssignMovementItemClientRpc(ulong networkObjectId, ulong ownerClientId)
    {
        if (!IsOwner) return; // Only run for the owning client

        NetworkObject spawnedObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        movementItem = spawnedObject.GetComponent<BaseItem>();
        movementItem.Initialize(player, player.OwnerClientId);
    }
    
    private void UseAttackItem()
    {
        if (!attackItem) return;
        attackItem.ActivateItem();
        attackItemImage.sprite = null;
    }

    private void UseMovementItem()
    {
        if (!movementItem) return;
        movementItem.ActivateItem();
        movementItemImage.sprite = null;
        DespawnMovementItemServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnMovementItemServerRpc()
    {
        movementItem.GetComponent<NetworkObject>().Despawn();
    }

    public void ClearItems()
    {
        Destroy(attackItem);
        Destroy(movementItem);

        attackItem = null;
        movementItem = null;
        
        attackItemImage.sprite = null;
        movementItemImage.sprite = null;
    }


    
    

    // [ServerRpc(RequireOwnership = false)]
    // private void ActivateAttackItemServerRpc()
    // {
    //     if (attackItem != null)
    //     {
    //         attackItem.ActivateItem();
    //     }
    //     
    // }
    //
    // [ServerRpc(RequireOwnership = false)]
    // private void ActivateMovementItemServerRpc()
    // {
    //     if (movementItem != null)
    //     {
    //         movementItem.ActivateItem();
    //     }
    //     
    // }
    // private void SelectAttackItem()
    // {
    //     if (attackItem != null) return;
    //     
    //     int itemIndex = Random.Range(0, attackItemPool.items.Length);
    //     ItemSO selectedItem = attackItemPool.items[itemIndex];
    //     attackItemImage.sprite = selectedItem.PowerupIcon;
    //     SpawnAttackItemServerRpc(itemIndex);
    //
    // }
    //
    // public void SelectItem(ItemType itemType)
    // {
    //     if (itemType == ItemType.Attack)
    //     {
    //         SelectAttackItem();
    //     }
    //     else
    //     {
    //         SelectMovementItem();
    //     }
    // }
    //
    // [ServerRpc(RequireOwnership = false)]
    // public void SpawnAttackItemServerRpc(int itemIndex, ServerRpcParams rpcParams = default)
    // {
    //     ulong ownerClientId = rpcParams.Receive.SenderClientId;
    //     ItemSO selectedItem = attackItemPool.items[itemIndex];
    //     if (selectedItem != null)
    //     {
    //         attackItem = Instantiate(selectedItem.prefab, itemParent).GetComponent<BaseItem>();
    //         attackItemImage.sprite = selectedItem.PowerupIcon;
    //         attackItem.Player = GetComponent<Player>();
    //         attackItem.GetComponent<NetworkObject>().Spawn();
    //     }
    //     if (movementItem != null)
    //     {
    //         attackItem.transform.localPosition = new Vector3(-movementItem.transform.localPosition.x, attackItem.transform.localPosition.y, attackItem.transform.localPosition.z);
    //     }
    //     else
    //     {
    //         attackItem.transform.localPosition = new Vector3(orbitRadius, 0, 0);
    //     }
    // }
    //
    // [ServerRpc(RequireOwnership = false)]
    // public void SpawnMovementItemServerRpc(int itemIndex, ServerRpcParams rpcParams = default)
    // {
    //     ulong ownerClientId = rpcParams.Receive.SenderClientId;
    //     ItemSO selectedItem = movementItemPool.items[itemIndex];
    //     if (selectedItem != null)
    //     {
    //         movementItem = Instantiate(selectedItem.prefab, itemParent).GetComponent<BaseItem>();
    //         movementItemImage.sprite = selectedItem.PowerupIcon;
    //         movementItem.Player = GetComponent<Player>();
    //         movementItem.GetComponent<NetworkObject>().Spawn();
    //
    //     }
    //
    //     if (attackItem != null)
    //     {
    //         movementItem.transform.localPosition = new Vector3(-attackItem.transform.localPosition.x, movementItem.transform.localPosition.y, movementItem.transform.localPosition.z);
    //     }
    //     else
    //     {
    //         movementItem.transform.localPosition = new Vector3(-orbitRadius, 0, 0);
    //     }
    // }
    //
    // public void ClearItems()
    // {
    //     Destroy(movementItem?.gameObject); 
    //     Destroy(attackItem?.gameObject);
    //     movementItem = null;
    //     attackItem = null;
    // }
    //
    // private void SelectMovementItem()
    // {
    //     if (movementItem != null) return;
    //     
    //     int itemIndex = Random.Range(0, movementItemPool.items.Length);
    //     ItemSO selectedItem = movementItemPool.items[itemIndex];
    //     movementItemImage.sprite = selectedItem.PowerupIcon;
    //     SpawnMovementItemServerRpc(itemIndex);
    //
    //
    // }
}