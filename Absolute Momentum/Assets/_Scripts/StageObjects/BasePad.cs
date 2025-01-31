using System;
using UnityEngine;


/// <summary>
/// This class should be used for all "Jump Pad" like objects
/// </summary>
public class BasePad : MonoBehaviour
{
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ActivatePad(other);
        }
    }

    public virtual void ActivatePad(Collision other)
    {
        
    }
}
