using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerItemManager : NetworkBehaviour
{
    [SerializeField] private BaseItem attackItem, movementItem;
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

        if (attackItem != null && Input.GetKeyDown(KeyCode.Mouse0))
        {
            attackItem.ActivateItem();
        }
        
        if (movementItem != null && Input.GetKeyDown(KeyCode.Mouse1))
        {
            movementItem.ActivateItem();
        }
        
    }
    private void SelectAttackItem()
    {
        if (attackItem != null) return;
        
        int randomItemNum = Random.Range(0, attackItemPool.items.Length);
        ItemSO selectedItem = attackItemPool.items[randomItemNum];
        if (selectedItem != null)
        {
            attackItem = Instantiate(selectedItem.prefab, itemParent).GetComponent<BaseItem>();
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
        
        int randomItemNum = Random.Range(0, movementItemPool.items.Length);
        ItemSO selectedItem = movementItemPool.items[randomItemNum];
        if (selectedItem != null)
        {
            movementItem = Instantiate(selectedItem.prefab, itemParent).GetComponent<BaseItem>();
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
}