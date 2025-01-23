using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSequence : MonoBehaviour
{
    public List<Dialogue> dialogues;

    public void OnEnable()
    {
        StartDialogueSequence();
    }

    public void StartSelf()
    {
        StartDialogueSequence();
    }

    private void StartDialogueSequence()
    {
        DialogueManager.Instance.StartDialogueSequence(this);
    }

    public void ContinueDialogue()
    {
        DialogueManager.Instance.DisplayNextSentence();
    }

    public void SwitchActionMapCutscene()
    {
        //InputManager.Instance.SwitchActionMap("Cutscene");
    }

    public void SwitchActionMapPlayer()
    {
        //InputManager.Instance.SwitchActionMap("Player");
    }
}
