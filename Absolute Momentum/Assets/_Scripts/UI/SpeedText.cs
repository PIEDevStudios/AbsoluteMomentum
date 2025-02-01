using TMPro;
using UnityEngine;

public class SpeedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Player player;

    // Update is called once per frame
    void FixedUpdate()
    {
        speedText.text = "X Speed: " +
                         new Vector3(player.rb.linearVelocity.x, 0, player.rb.linearVelocity.z).magnitude.ToString("F2") +
                         "m/s\nY Speed: " + player.rb.linearVelocity.y.ToString("F2") + "m/s";
    }
}
