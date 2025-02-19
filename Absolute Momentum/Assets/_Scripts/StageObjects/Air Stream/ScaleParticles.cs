using UnityEngine;

public class ScaleParticles : MonoBehaviour
{
    public Transform parent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.localScale = new Vector3(target.localScale.x, 1, target.localScale.z);
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var shape = ps.shape;
        shape.scale = new Vector3(parent.localScale.x, 1, parent.localScale.z);
    }
}
