using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : SingletonPersistent<SceneTransitionManager>
{
    [SerializeField] Animator fadeTransition;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(StartTransition(sceneName));
    }
    IEnumerator StartTransition(string sceneName) {
        fadeTransition.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
        fadeTransition.SetTrigger("Start");
    }
}
