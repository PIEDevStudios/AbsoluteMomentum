using UnityEngine;

[CreateAssetMenu(fileName = "ItemPickupSO", menuName = "Scriptable Objects/ItemPickupSO")]
public class ItemPickupSO : ScriptableObject
{
    public string powerupName;
    public string powerupDescrip;
    public GameObject powerup;

}
