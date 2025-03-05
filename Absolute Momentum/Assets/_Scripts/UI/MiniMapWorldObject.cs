using System;
using Unity.VisualScripting;
using UnityEngine;

public class MiniMapWorldObject : MonoBehaviour
{
    public Sprite Icon;
    public Color IconColor = Color.white;
    public string Text;
    public int TextSize = 10;

    private void Start()
    {
        MiniMap.Instance.RegisterMiniMapWorldObject(this);
    }
}
