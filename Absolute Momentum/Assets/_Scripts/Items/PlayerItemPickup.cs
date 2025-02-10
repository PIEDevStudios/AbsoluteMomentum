using UnityEngine;
using Random = System.Random;
public class PlayerItemPickup : MonoBehaviour
{
    [SerializeField] private ItemPoolSO itemPool;

    void Update() {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectItem();
        }   
    }
    private void SelectItem()
    {
        Random rnd = new Random();  
        int randomItemNum = rnd.Next(itemPool.items.Length);
        ItemPickupSO selectedItem = itemPool.items[randomItemNum];
        Instantiate(selectedItem.powerup);
    }
}