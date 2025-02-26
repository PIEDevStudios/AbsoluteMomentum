using UnityEngine;

public class SmokeBombProjectile : MonoBehaviour
{
    public ParticleSystem particles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) {
            particles.Play();

            Destroy(gameObject, 2f);
        }


    }
    
}
