using Unity.Netcode;
using UnityEngine;

public class BaseItem : NetworkBehaviour
{
    [HideInInspector] public Player Player;
    public virtual void ActivateItem() { }
}
