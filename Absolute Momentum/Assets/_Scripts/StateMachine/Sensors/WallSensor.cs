using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 3D wall sensor
/// </summary>
public class WallSensor : MonoBehaviour
{
    [SerializeField] private Player player;
    public bool wallLeft { get; private set; }
    public bool wallRight { get; private set; }
    public RaycastHit wallHitLeft;
    public RaycastHit wallHitRight;
    [SerializeField] private float wallCheckDistance;
    public LayerMask wallLayer;
    public GroundSensor minHeightSensor;

    private void Update()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        wallRight = Physics.Raycast(transform.position, player.playerObj.right, out wallHitRight, wallCheckDistance, wallLayer);
        Debug.DrawRay(transform.position, player.playerObj.right * wallCheckDistance);
        wallLeft = Physics.Raycast(transform.position, -player.playerObj.right, out wallHitLeft, wallCheckDistance, wallLayer);
        Debug.DrawRay(transform.position, -player.playerObj.right * wallCheckDistance);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * wallCheckDistance);
    }
}
