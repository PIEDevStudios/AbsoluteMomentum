using UnityEngine;

public class PlayerFootstepParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem footstepParticle;
    [SerializeField] private Transform footTransformR, footTransformL;
    [SerializeField] private Player player; // Optional: If you want to check grounded or running

    public void PlayFootstepR()
    {
        Debug.Log("PlayFootstepR");
        
        if (!player.groundSensor.grounded || !player.stateMachine.currentState == player.move || !player.stateMachine.currentState == player.wallrun)
            return;

        // Move particle to foot and play
        footstepParticle.transform.position = footTransformR.position;
        footstepParticle.Play();
    }
    
    public void PlayFootstepL()
    {
        Debug.Log("PlayFootstepL");
        
        
        if (!player.groundSensor.grounded || !player.stateMachine.currentState == player.move || !player.stateMachine.currentState == player.wallrun)
            return;

        // Move particle to foot and play
        footstepParticle.transform.position = footTransformL.position;
        footstepParticle.Play();
    }
}