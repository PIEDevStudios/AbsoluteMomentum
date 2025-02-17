using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public virtual void Start()
    {
        HoldItem();
    }
    public virtual void HoldItem() { }
    public virtual void ActivateItem() { }
}
