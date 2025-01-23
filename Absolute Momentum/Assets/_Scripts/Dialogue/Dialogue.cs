using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    //public string name;                 // Name of NPC
    //public GameObject dialogueBox;
    //public TextMeshProUGUI dialogueText;
    //public TextMeshProUGUI nameText;
    public Sprite characterSprite;
   // public Image characterImage;
    public UnityEvent actionOnComplete;
    public bool skippableByClick;

    [TextArea(3, 10)]
    public string[] sentences;          // All of the sentences the NPC will say

}