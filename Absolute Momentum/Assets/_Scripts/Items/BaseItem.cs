using Unity.Netcode;
using UnityEngine;

public class BaseItem : NetworkBehaviour
{
    [HideInInspector] public Player Player;
    [HideInInspector] public ulong ItemUserClientId;
    public virtual void ActivateItem() { }
}
