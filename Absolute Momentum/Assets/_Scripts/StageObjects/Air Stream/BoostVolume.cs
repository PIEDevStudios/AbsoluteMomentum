using System;
using Unity.VisualScripting;
using UnityEngine;

public class BoostVolume : MonoBehaviour
{
    [SerializeField] private float intensity;
    [SerializeField] private Direction boostDirection;

    // FOR GIZMO
    private float length = 4f;                   // How long the line is
    private float arrowHeadLength = 0.25f;       // Size of arrowhead
    private float arrowHeadAngle = 20f;          // Angle of the arrowhead
    private float offset = 1f;          // Angle of the arrowhead
    
    public enum Direction
    {
        Up,
        Forward,
        Right,
        Left,
        Back,
        Down
    }

    public Vector3 getDirection(Direction direction)
    {
        if (direction == Direction.Up)
        {
            return transform.up;
        }
        if (direction == Direction.Forward)
        {
            return transform.forward;
        }
        if (direction == Direction.Right)
        {
           return transform.right; 
        }
        if (direction == Direction.Left)
        {
            return -transform.right; 
        }
        if (direction == Direction.Back)
        {
            return -transform.forward; 
        }
        return Vector3.down;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.attachedRigidbody;
            Vector3 direction = getDirection(boostDirection);
            direction *= intensity;
            playerRigidbody.AddForce(direction, ForceMode.Force);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = getDirection(boostDirection);
        
        // Start and end points
        Vector3 startPos = transform.position + transform.up * offset;
        Vector3 endPos = startPos + direction.normalized * length;

        // Draw main line
        Gizmos.DrawLine(startPos, endPos);

        // Draw arrowhead
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;

        Gizmos.DrawLine(endPos, endPos + right * arrowHeadLength);
        Gizmos.DrawLine(endPos, endPos + left * arrowHeadLength);
    }
}
