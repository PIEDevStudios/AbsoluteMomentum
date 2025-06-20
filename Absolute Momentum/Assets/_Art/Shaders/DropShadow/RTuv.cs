using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTuv : MonoBehaviour
{
    [SerializeField]
    Material material;
    [SerializeField]
    Transform target;
    private void Start()
    {
        material.SetFloat("_OrthographicCamSize", GetComponent<Camera>().orthographicSize);
    }
    private void Update()
    {
        material.SetVector("_PlayerPosition", new Vector3(target.position.x, target.position.y, target.position.z));
    }
}
