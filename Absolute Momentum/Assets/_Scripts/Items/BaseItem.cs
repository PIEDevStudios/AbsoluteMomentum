using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseItem : NetworkBehaviour
{
    [HideInInspector] public Player Player;
    [FormerlySerializedAs("ItemUserClientId")] [HideInInspector] public ulong ItemUserClientID;

    public void Initialize(Player player, ulong itemUserClientId)
    {
        Player = player;
        ItemUserClientID = itemUserClientId;
    }
    public virtual void ActivateItem() { }
}
