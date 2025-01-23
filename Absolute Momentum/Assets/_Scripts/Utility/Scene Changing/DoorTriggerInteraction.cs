using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractionBase
{
    [SerializeField] private bool autoTransition = false;

    public enum DoorToSpawnAt
    {
        None,
        One,
        Two,
        Three,
    }

    [Header("Spawn TO")]
    [SerializeField] private DoorToSpawnAt doorToSpawnAt;
    [SerializeField] private SceneField sceneToLoad;

    [Space(10f)]
    public DoorToSpawnAt currentDoor;

    public override void Interact()
    {
        base.Interact();
        Debug.Log("Use Door");
        SceneSwapManager.Instance.SwapSceneFromDoorUse(sceneToLoad, doorToSpawnAt);
        //InputManager.Instance.EnablePlayerInput(false);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (!autoTransition) return;

        if (collision.CompareTag("Player"))
        {
            SceneSwapManager.Instance.SwapSceneFromDoorUse(sceneToLoad, doorToSpawnAt);
            //InputManager.Instance.EnablePlayerInput(false);
        }
    }
}
