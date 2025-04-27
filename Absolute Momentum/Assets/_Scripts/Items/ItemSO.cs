using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemPickupSO")]
public class ItemSO : ScriptableObject
{
    public string powerupName;
    public string powerupDescrip;
    [FormerlySerializedAs("powerup")] public GameObject prefab;

}
