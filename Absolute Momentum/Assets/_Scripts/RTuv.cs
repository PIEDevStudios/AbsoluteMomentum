using UnityEngine;

public class RTuv : MonoBehaviour
{
    [SerializeField]
    Material material;
    [SerializeField]
    Transform target;
 
    private void Update()
    {
        material.SetVector("_PlayerPosition", new Vector2(target.position.x, target.position.z));
    }
}
