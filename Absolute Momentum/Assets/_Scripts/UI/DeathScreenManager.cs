using UnityEngine;

public class DeathScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject[] deathScreensPrefabs; // Assign prefabs
    private GameObject activeScreen;

    public void PlayDeathScreen(int index)
    {
        Debug.Log("DEATH SCREEN");
        if (index < 0 || index >= deathScreensPrefabs.Length) return;

        if (activeScreen != null)
            Destroy(activeScreen.gameObject);

        activeScreen = Instantiate(deathScreensPrefabs[index], transform);
        activeScreen.GetComponent<BaseDeathScreen>().Play();
    }
}
