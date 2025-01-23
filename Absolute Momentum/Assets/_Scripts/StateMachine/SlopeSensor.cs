using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class SlopeSensor : MonoBehaviour
{
    [SerializeField] private float rayLength;
    [SerializeField] private float maxSlopeAngle = 60f; // Maximum angle for a surface to be considered a slope, if a surface has a greater angle than this number then it will not be walkable
    [SerializeField] private float minSlopeAngle = 10f; // Minimum angle for a surface to be considered a slope, if a surface has an angle smaller than this number, then it will be considered regular ground
    [field:ReadOnly, SerializeField] public float currentSlopeAngle {get; private set;}
    public LayerMask groundLayer;
    public bool isOnSlope { get; private set; }
    public RaycastHit hit;
    // TODO: Add maximum slope angle check
    private void Update()
    {
        CheckSlope();
    }

    private void CheckSlope()
    {
        bool raycast = Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, groundLayer);
        currentSlopeAngle = Mathf.Acos(Vector3.Dot(hit.normal, Vector3.up)) * Mathf.Rad2Deg;
        isOnSlope = raycast && currentSlopeAngle < maxSlopeAngle && currentSlopeAngle > minSlopeAngle;
        Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.red);
    }
}
