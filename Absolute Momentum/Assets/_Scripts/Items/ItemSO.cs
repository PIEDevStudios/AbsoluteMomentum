using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemPickupSO")]
public class ItemSO : ScriptableObject
{
    public string powerupName;
    public string powerupDescrip;
    public Sprite PowerupIcon;
    [FormerlySerializedAs("powerup")] public GameObject prefab;

}
