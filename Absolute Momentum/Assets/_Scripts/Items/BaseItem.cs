using Unity.Netcode;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public Player player;
    public virtual void ActivateItem() { }
}
