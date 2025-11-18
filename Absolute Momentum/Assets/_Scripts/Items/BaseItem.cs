using Unity.Netcode;
using UnityEngine;

public class BaseItem : NetworkBehaviour
{
    public Player player;
    public virtual void ActivateItem() { }
}
