using TMPro;
using UnityEngine;

public class SpeedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Player player;

    // Update is called once per frame
    void Update()
    {
        speedText.text = "X Speed: " +
                         new Vector3(player.rb.velocity.x, 0, player.rb.velocity.z).magnitude.ToString("F2") +
                         "m/s\nY Speed: " + player.rb.velocity.y.ToString("F2") + "m/s";
    }
}
