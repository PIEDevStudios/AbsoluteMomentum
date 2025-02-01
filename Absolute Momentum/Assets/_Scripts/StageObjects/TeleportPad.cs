using System;
using UnityEngine;

public class TeleportPad : MonoBehaviour
{
    [SerializeField] private Transform _teleportPoint;

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.position = _teleportPoint.position;
        }
    }
}
