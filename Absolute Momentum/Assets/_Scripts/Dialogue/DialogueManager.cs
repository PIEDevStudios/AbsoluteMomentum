using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This script manages ALL dialog in this scene

public class DialogueManager : Singleton<DialogueManager>
{
    private Dialogue currentDialogue;

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshPro dialogueText;
    [SerializeField] private GameObject characterImage;
    [SerializeField] private AudioSource talkingAudio;
    private SpriteRenderer characterRend;
    //private InputManager inputManager;
    //private StudioEventEmitter eventEmitter;
    private DialogueSequence currentSequence;
    private int currentSequenceIndex;


    [SerializeField] private float textDelay;
    private Queue<string> sentences;

    protected override void Awake()
    {
        base.Awake();
        sentences = new Queue<string>();
        characterRend = characterImage.GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }


    private void Start()
    {
        //inputManager = InputManager.Instance;
    }

    private void OnEnable()
    {
        //inputManager = InputManager.Instance;
    }

    private void Update()
    {
        //Debug.Log(inputManager == null);
        //Debug.Log("Pressed: " + inputManager.NextDialoguePressedThisFrame);
        if (Input.GetMouseButtonDown(0))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogueSequence(DialogueSequence sequence)
    {
        currentSequenceIndex = 0;
        currentSequence = sequence;
        StartDialogue(sequence.dialogues[currentSequenceIndex]);
    }

    private void NextDialogueInSequence()
    {
        currentSequenceIndex++;
        if(currentSequenceIndex < currentSequence.dialogues.Count)
        {
            StartDialogue(currentSequence.dialogues[currentSequenceIndex]);
        }
        else
        {
            EndDialogueSequence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        //InputManager.Instance.playerInput.SwitchCurrentActionMap("Cutscene");

        //AudioManager.Instance.PlayOneShot(FMODEvents.NetworkSFXName.DialogueComplete, transform.position);

        currentDialogue = dialogue;

        dialogueBox.SetActive(true);
        

        if (characterImage != null)
        {
            if(dialogue.characterSprite == null)
            {
                characterImage.SetActive(false);
            }

            else
            {
                characterImage.SetActive(true);
                characterRend.sprite = dialogue.characterSprite;
            }
            
        }
            

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();

    }

    public void DisplayNextSentence()
    {
        
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        //eventEmitter?.Stop();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        talkingAudio.Play();
        dialogueText.text = "";
        //eventEmitter = AudioManager.Instance.InitializeEventEmitter(FMODEvents.NetworkSFXName.DialogueTalk, gameObject);
        //eventEmitter.Play();
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textDelay);
        }
        talkingAudio.Stop();
        //eventEmitter.Stop();
    }

    public void EndDialogue()
    {
        currentDialogue?.actionOnComplete.Invoke();
        currentDialogue = null;

        if (currentSequence != null)
        {
            NextDialogueInSequence();
        }
        else
        {
            Debug.Log("End of Dialogue");
            dialogueBox.SetActive(false);
        }
    }

    private void EndDialogueSequence()
    {
        Debug.Log("End of Dialogue Sequence");
        dialogueBox.SetActive(false);
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