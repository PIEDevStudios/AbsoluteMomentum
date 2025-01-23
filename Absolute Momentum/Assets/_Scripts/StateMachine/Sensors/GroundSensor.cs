using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    [SerializeField] private float rayLength;
    public LayerMask groundLayer;
    public bool grounded { get; private set; }

    private void Update()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, rayLength, groundLayer);
        Debug.DrawRay(transform.position, Vector3.down * rayLength);
    }
}
