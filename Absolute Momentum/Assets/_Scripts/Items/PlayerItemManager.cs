using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerItemManager : NetworkBehaviour
{
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
    void Update()
    {
        if (!IsOwner) return;
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectAttackItem();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectMovementItem();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActivateAttackItemServerRpc();
            attackItemImage.sprite = null; 
            Debug.Log($"activate attack");
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {

            ActivateMovementItemServerRpc();
            movementItemImage.sprite = null;
            Debug.Log($"activate move");

        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void ActivateAttackItemServerRpc()
    {
        if (attackItem != null)
        {
            attackItem.ActivateItem();
        }
        
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ActivateMovementItemServerRpc()
    {
        if (movementItem != null)
        {
            movementItem.ActivateItem();
        }
        
    }
    private void SelectAttackItem()
    {
        if (attackItem != null) return;
        
        int itemIndex = Random.Range(0, attackItemPool.items.Length);
        ItemSO selectedItem = attackItemPool.items[itemIndex];
        attackItemImage.sprite = selectedItem.PowerupIcon;
        SpawnAttackItemServerRpc(itemIndex);

    }

    public void SelectItem(ItemType itemType)
    {
        if (itemType == ItemType.Attack)
        {
            SelectAttackItem();
        }
        else
        {
            SelectMovementItem();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnAttackItemServerRpc(int itemIndex)
    {
        ItemSO selectedItem = attackItemPool.items[itemIndex];
        if (selectedItem != null)
        {
            attackItem = Instantiate(selectedItem.prefab, itemParent).GetComponent<BaseItem>();
            attackItemImage.sprite = selectedItem.PowerupIcon;
            attackItem.Player = GetComponent<Player>();
            attackItem.GetComponent<NetworkObject>().Spawn();
        }
        if (movementItem != null)
        {
            attackItem.transform.localPosition = new Vector3(-movementItem.transform.localPosition.x, attackItem.transform.localPosition.y, attackItem.transform.localPosition.z);
        }
        else
        {
            attackItem.transform.localPosition = new Vector3(orbitRadius, 0, 0);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnMovementItemServerRpc(int itemIndex)
    {
        ItemSO selectedItem = movementItemPool.items[itemIndex];
        if (selectedItem != null)
        {
            movementItem = Instantiate(selectedItem.prefab, itemParent).GetComponent<BaseItem>();
            movementItemImage.sprite = selectedItem.PowerupIcon;
            movementItem.Player = GetComponent<Player>();
            movementItem.GetComponent<NetworkObject>().Spawn();

        }

        if (attackItem != null)
        {
            movementItem.transform.localPosition = new Vector3(-attackItem.transform.localPosition.x, movementItem.transform.localPosition.y, movementItem.transform.localPosition.z);
        }
        else
        {
            movementItem.transform.localPosition = new Vector3(-orbitRadius, 0, 0);
        }
    }
    
    public void ClearItems()
    {
        Destroy(movementItem?.gameObject); 
        Destroy(attackItem?.gameObject);
        movementItem = null;
        attackItem = null;
    }
    
    private void SelectMovementItem()
    {
        if (movementItem != null) return;
        
        int itemIndex = Random.Range(0, movementItemPool.items.Length);
        ItemSO selectedItem = movementItemPool.items[itemIndex];
        movementItemImage.sprite = selectedItem.PowerupIcon;
        SpawnMovementItemServerRpc(itemIndex);


    }
}