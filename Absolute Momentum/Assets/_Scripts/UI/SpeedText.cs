using TMPro;
using UnityEngine;

public class SpeedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Player player;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 flatVel = new Vector3(player.rb.linearVelocity.x, 0, player.rb.linearVelocity.z);
        speedText.text = "Speed: " +
                         flatVel.magnitude.ToString("F2") +
                         "m/s\nV Speed: " + player.rb.linearVelocity.y.ToString("F2") + "m/s";
    }
}
