using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerItemManager : NetworkBehaviour
{
    [SerializeField] private GameObject attackItem, movementItem;
    
    [SerializeField] private ItemPoolSO itemPool;
    [SerializeField] private Transform itemParent; // The parent transform for the items
    void Update()
    {
        if (!IsOwner) return;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectItem();
        }   
    }
    private void SelectItem()
    {
        int randomItemNum = Random.Range(0, itemPool.items.Length);
        ItemPickupSO selectedItem = itemPool.items[randomItemNum];
        if (selectedItem != null)
        {
            Instantiate(selectedItem.powerup, itemParent);
        }

    }
}