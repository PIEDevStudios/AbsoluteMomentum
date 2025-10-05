using UnityEngine;

public class VaultSensor : MonoBehaviour
{
    [SerializeField] private float ledgeCheckDistance = 0.6f;
    [SerializeField] private float ledgeHeight = 1.4f;
    [SerializeField] private float ledgeLowerHeight = 0.5f;
    [SerializeField] private LayerMask ledgeLayer;

    public bool CheckForLedge(out RaycastHit ledgeHit)
    {
        ledgeHit = new RaycastHit();

        Vector3 originUpper = transform.position + Vector3.up * ledgeHeight;
        Vector3 originLower = transform.position + Vector3.up * ledgeLowerHeight;

        // forward ray at head height
        if (Physics.Raycast(originLower, transform.forward, out RaycastHit hitInfo, ledgeCheckDistance, ledgeLayer))
        {
            // player is close enough to grab the top of the ledge
            if (!Physics.Raycast(originUpper, transform.forward, ledgeCheckDistance, ledgeLayer))
            {
                ledgeHit = hitInfo;
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 originUpper = transform.position + Vector3.up * ledgeHeight;
        Vector3 originLower = transform.position + Vector3.up * ledgeLowerHeight;
        Gizmos.DrawRay(originUpper, transform.forward * ledgeCheckDistance);
        Gizmos.DrawRay(originLower, transform.forward * ledgeCheckDistance);
    }

}
