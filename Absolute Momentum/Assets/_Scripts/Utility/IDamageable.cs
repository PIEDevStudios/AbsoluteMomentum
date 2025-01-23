using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage) {}

    void TakeDamage (int damage,int invicibilityFrames) {}

    void Die() {}
}
