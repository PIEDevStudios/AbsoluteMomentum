using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class PlayerSpeedManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [ReadOnly] public AnimationCurve currentCurve;
    
    [SerializeField] private TextMeshProUGUI speedText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCurve = player.stats.airDragCurve;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 flatVel = new Vector3(player.rb.linearVelocity.x, 0, player.rb.linearVelocity.z);
        
        // If the player is on the slope, apply force on the plane of the slope instead
        if (player.slopeSensor.isOnSlope)
        {
            flatVel = Vector3.ProjectOnPlane(player.rb.linearVelocity, player.slopeSensor.hit.normal);
        }

        if (speedText != null)
        {
            speedText.text = flatVel.magnitude.ToString("0") + " mph";
        }
        
        float forceToApply = currentCurve.Evaluate(flatVel.magnitude);
        
        player.rb.AddForce(forceToApply * -flatVel.normalized, ForceMode.Force);
        
    }
    
    
    
    
}
