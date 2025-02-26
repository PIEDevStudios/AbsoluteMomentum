using System;
using Unity.VisualScripting;
using UnityEngine;

public class AirStream : MonoBehaviour
{
    [SerializeField] private float intensity;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.attachedRigidbody;
            Vector3 direction = gameObject.transform.up;
            direction *= intensity;
            playerRigidbody.AddForce(direction, ForceMode.Force);
        }

        // if (other.attachedRigidbody)
        // {
        //     
        //     other.attachedRigidbody.AddForce(Vector3.up * 10);
        // }
    }
}
