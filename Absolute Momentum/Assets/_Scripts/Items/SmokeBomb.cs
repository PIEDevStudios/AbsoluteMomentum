using System;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

public class SmokeBomb : BaseItem
{
    private bool ItemHeld = false;
    public Player player;
    public float orbitRadius = 3f;
    public float orbitDuration = 5f;
    public float throwForce = 70f;
    public float throwUpwardForce = 10f;
    public GameObject item;

    public override void Start()
    {
        player = GetComponentInParent<Player>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && ItemHeld)
        {
            ActivateItem();
        }
    }

    public override void HoldItem()
    {
        base.HoldItem();
        ItemHeld = true;

        transform.position = player.transform.position + new Vector3(orbitRadius, 0, 0);
        
    }

    public override void ActivateItem()
    {
        base.ActivateItem();
        ItemHeld = false;

        GameObject projectile = Instantiate(item, player.transform.position, player.orientation.rotation);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceToAdd = player.orientation.forward * throwForce + transform.up  * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        Destroy(gameObject);
    }
}
