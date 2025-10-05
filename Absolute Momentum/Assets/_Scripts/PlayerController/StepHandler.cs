using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StepHandler : MonoBehaviour
{
    [field: SerializeField] public float RayDistance { get; private set; }
    [field: SerializeField] public float MaxStepHeight { get; private set; }
    [field: SerializeField] public float MaxLerp { get; private set; } = 0.5f;
    [field: SerializeField] public Transform Orientation { get; private set; }
    [field: SerializeField] public LayerMask GroundLayer { get; private set; }
    
    private Rigidbody _playerRb;
    private CapsuleCollider _playerCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _playerRb = GetComponent<Rigidbody>();
        _playerCollider = GetComponentInChildren<CapsuleCollider>();
    }

    void Update()
    {
        Vector3 originBottom = transform.position + (-_playerCollider.height / 2 + 0.1f) * Vector3.up;
        Vector3 originTop = transform.position + (MaxStepHeight - _playerCollider.height / 2) * Vector3.up;

        bool bottomHit = Physics.Raycast(originBottom, _playerRb.linearVelocity.normalized, out RaycastHit bottomInfo, RayDistance, GroundLayer);
        bool topHit = Physics.Raycast(originTop, _playerRb.linearVelocity.normalized, out _, RayDistance, GroundLayer);

        Debug.DrawRay(originBottom, _playerRb.linearVelocity.normalized * RayDistance, bottomHit ? Color.red : Color.white);
        Debug.DrawRay(originTop, _playerRb.linearVelocity.normalized * RayDistance, topHit ? Color.red : Color.white);

        if (bottomHit && !topHit)
        {
            
            // Check if we’re moving forward
            Vector3 flatVel = new Vector3(_playerRb.linearVelocity.x, 0, _playerRb.linearVelocity.z);
            if (flatVel.magnitude < 0.5f)
                return;
            
            // Cast down from above the step to find ground
            Vector3 stepOrigin = bottomInfo.point + Vector3.up * MaxStepHeight + _playerRb.linearVelocity.normalized * 0.05f;
            if (Physics.Raycast(stepOrigin, Vector3.down, out RaycastHit stepHit, MaxStepHeight * 1.5f, GroundLayer))
            {
                float heightDiff = stepHit.point.y - transform.position.y + _playerCollider.height / 2;
                // Only step up if it’s within valid range
                if (heightDiff > 0.05f && heightDiff <= MaxStepHeight)
                {
                    // Check slope angle (avoid walls)
                    if (Vector3.Dot(stepHit.normal, Vector3.up) < 0.99f) return;
                    Debug.Log("STEPPING UP");
                    
                    // Smoothly lift player instead of teleporting
                    Vector3 targetPos = new Vector3(_playerRb.position.x, stepHit.point.y + _playerCollider.height / 2 + 0.1f, _playerRb.position.z);
                    _playerRb.MovePosition(Vector3.Lerp(_playerRb.position, targetPos, MaxLerp * Time.deltaTime));

                    // Prevent falling/bumping immediately after stepping
                    _playerRb.linearVelocity = new Vector3(_playerRb.linearVelocity.x, 0, _playerRb.linearVelocity.z);
                }
            }
        }
    }
}

