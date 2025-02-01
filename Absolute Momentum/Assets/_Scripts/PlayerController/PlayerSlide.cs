using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerSlide : State
{
    [SerializeField] private Player player;
    [SerializeField] private Transform orientation, colliderPivot, graphics;
    private Vector3 enterHitboxScale;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        enterHitboxScale = colliderPivot.localScale;
        colliderPivot.localScale = player.stats.slidePlayerScale;
        graphics.localScale = player.stats.slidePlayerScale;
        
        // Add force down to stick the player to the ground
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    public override void DoUpdateState()
    {
        base.DoUpdateState();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        colliderPivot.localScale = enterHitboxScale;
        graphics.localScale = enterHitboxScale;
    }
}
