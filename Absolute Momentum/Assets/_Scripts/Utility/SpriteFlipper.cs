using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    [Tooltip("Check this box if sprite is initially facing right")]
    [field: SerializeField] public bool isFacingRight { get; private set; }
    public Rigidbody2D rb;

    /// <summary>
    /// Flips the direction of the sprite.
    /// </summary>
    /// <param name="direction"></param>
    public void FlipSprite()
    {
        if (transform.eulerAngles.y == 180)   // Facing right
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 180, transform.rotation.z));
        }

        isFacingRight = !isFacingRight;

    }

    public void CheckSpriteDirection()
    {
        if (rb.linearVelocity.x > 0 && !isFacingRight)
        {
            FlipSprite();
        }
        else if (rb.linearVelocity.x < 0 && isFacingRight)
        {
            FlipSprite();
        }
    }
}
