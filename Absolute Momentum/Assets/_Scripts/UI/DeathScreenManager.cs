using UnityEngine;

public class DeathScreenManager : MonoBehaviour
{
    [SerializeField] private BaseDeathScreen[] deathScreens; // Assign prefabs
    private BaseDeathScreen activeScreen;

    public void PlayDeathScreen(int index)
    {
        if (index < 0 || index >= deathScreens.Length) return;

        if (activeScreen != null)
            Destroy(activeScreen.gameObject);

        activeScreen = Instantiate(deathScreens[index], transform);
        activeScreen.Play();
    }
}
