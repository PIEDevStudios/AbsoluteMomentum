using UnityEngine;
using System;
using System.Collections.Generic;


public class PlayerItemDisplay : MonoBehaviour
{
    public Transform itemParent;
    public float orbitRadius;

    private GameObject attackVisual;
    private GameObject movementVisual;

    public void ShowAttackItem(ItemSO item)
    {
        if (attackVisual) Destroy(attackVisual);
        attackVisual = Instantiate(item.prefab, itemParent);
        attackVisual.transform.localPosition = new Vector3(orbitRadius, 0, 0);
    }

    public void ShowMovementItem(ItemSO item)
    {
        if (movementVisual) Destroy(movementVisual);
        movementVisual = Instantiate(item.prefab, itemParent);
        movementVisual.transform.localPosition = new Vector3(-orbitRadius, 0, 0);
    }

    public void Clear()
    {
        if (attackVisual) Destroy(attackVisual);
        if (movementVisual) Destroy(movementVisual);
    }
}
