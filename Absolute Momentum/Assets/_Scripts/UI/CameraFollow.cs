using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float followDist = 2.5f;
    [SerializeField] float elevation = 2.5f;
    [SerializeField] float angle = 20;
    [SerializeField] Transform target;

    private void Start()
    {
        transform.parent = target.transform;
        transform.localPosition = new Vector3(0, elevation, -followDist);
        transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }

}
