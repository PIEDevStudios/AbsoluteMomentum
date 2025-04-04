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
            Debug.Log("Activate Pad");
            
            ActivatePad(other.rigidbody);
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Activate Pad");
            ActivatePad(other.attachedRigidbody);
        }
    }

    public virtual void ActivatePad(Rigidbody other)
    {
        
    }
}
