 using System.Collections;
using System.Collections.Generic;
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
    public bool wallForward { get; private set; }
    public RaycastHit wallHitLeft;
    public RaycastHit wallHitRight;
    public RaycastHit wallHitForward;
    [SerializeField] private float wallCheckDistance;
    public LayerMask wallLayer;
    public GroundSensor minHeightSensor;

    private void Update()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        
        // Right Wall
        wallRight = Physics.Raycast(transform.position, player.playerObj.right, out wallHitRight, wallCheckDistance,
            wallLayer);

        Color color = wallRight? Color.red : Color.white;
        
        Debug.DrawRay(transform.position, player.playerObj.right * wallCheckDistance, color);
        
        // Left Wall
        wallLeft = Physics.Raycast(transform.position, -player.playerObj.right, out wallHitLeft, wallCheckDistance,
            wallLayer);
        
        color = wallLeft? Color.red : Color.white;
        
        Debug.DrawRay(transform.position, -player.playerObj.right * wallCheckDistance, color);
        
        
        // Forward wall
        wallForward = Physics.Raycast(transform.position, player.playerObj.forward, out wallHitForward, wallCheckDistance,
            wallLayer);
        
        color = wallForward? Color.red : Color.white;
        
        Debug.DrawRay(transform.position, player.playerObj.forward * wallCheckDistance, color);
        
    }
}
