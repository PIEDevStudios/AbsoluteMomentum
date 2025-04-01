using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ColliderScaler : MonoBehaviour
{
    [ContextMenu("Scale Collider to Fit Scene")]
    public void ScaleColliderToFitScene()
    {
        

        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("No Collider found on this GameObject.");
            return;
        }

        Bounds sceneBounds = CalculateSceneBounds();
        if (sceneBounds.size == Vector3.zero)
        {
            Debug.LogWarning("Scene bounds could not be calculated.");
            return;
        }
        // Reset collider before scaling
        ResetCollider(collider);
        
        // Set the collider's size and position
        if (collider is BoxCollider boxCollider)
        {
            boxCollider.center = sceneBounds.center - transform.position;
            boxCollider.size = sceneBounds.size;
        }
        else if (collider is SphereCollider sphereCollider)
        {
            sphereCollider.center = sceneBounds.center - transform.position;
            sphereCollider.radius = Mathf.Max(sceneBounds.extents.x, sceneBounds.extents.y, sceneBounds.extents.z);
        }
        else if (collider is CapsuleCollider capsuleCollider)
        {
            capsuleCollider.center = sceneBounds.center - transform.position;
            capsuleCollider.height = sceneBounds.size.y;
            capsuleCollider.radius = Mathf.Max(sceneBounds.extents.x, sceneBounds.extents.z);
        }

        Debug.Log("Collider scaled to fit scene bounds!");
    }

    private Bounds CalculateSceneBounds()
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        Collider[] colliders = FindObjectsOfType<Collider>();

        bool hasBounds = false;

        foreach (Renderer renderer in renderers)
        {
            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        foreach (Collider collider in colliders)
        {
            if (!hasBounds)
            {
                bounds = collider.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(collider.bounds);
            }
        }

        return bounds;
    }
    
    private void ResetCollider(Collider collider)
    {
        if (collider is BoxCollider boxCollider)
        {
            boxCollider.center = Vector3.zero;
            boxCollider.size = Vector3.one;
        }
        else if (collider is SphereCollider sphereCollider)
        {
            sphereCollider.center = Vector3.zero;
            sphereCollider.radius = 1f;
        }
        else if (collider is CapsuleCollider capsuleCollider)
        {
            capsuleCollider.center = Vector3.zero;
            capsuleCollider.height = 2f;
            capsuleCollider.radius = 0.5f;
        }
    }
}
