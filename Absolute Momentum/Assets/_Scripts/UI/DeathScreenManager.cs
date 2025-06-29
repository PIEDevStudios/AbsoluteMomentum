using UnityEngine;

public class DeathScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject[] deathScreensPrefabs; // Assign prefabs
    private GameObject activeScreen;

    public float PlayDeathScreen(int index)
    {
        Debug.Log("DEATH SCREEN");
        if (index < 0 || index >= deathScreensPrefabs.Length) return -1f;

        if (activeScreen != null)
            Destroy(activeScreen.gameObject);

        activeScreen = Instantiate(deathScreensPrefabs[index], transform);
        
        BaseDeathScreen deathScreen = activeScreen.GetComponent<BaseDeathScreen>();
        deathScreen.Play();
        return deathScreen.GetRespawnDelay();
    }

    public int GetNumberOfDeathScreens()
    {
        return deathScreensPrefabs.Length;
    }
}
