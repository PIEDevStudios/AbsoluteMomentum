using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    private GameObject cam;
    [SerializeField] private float parallaxeffect;

    void Start()
    {
        startpos = transform.position.x;
        cam = Camera.main.gameObject;
        //cam = GameObject.FindGameObjectWithTag("Player");
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxeffect));
        float dist = (cam.transform.position.x * parallaxeffect);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if (temp > startpos + length)
            startpos += length;
        else if (temp < startpos - length) 
            startpos -= length;
    }
}
