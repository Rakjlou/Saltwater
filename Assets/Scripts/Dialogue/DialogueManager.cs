using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Configuration")]
    public TextMeshProUGUI textMesh;
    public Color textColor;

    [Header("Dialogue")]
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    private Coroutine typingCoroutine = null;
    private int lineIndex = 0;

    void Awake()
    {
        textMesh.color = textColor;
        textMesh.text = "";
    }

    void Start()
    {
        DisplayDialogueLines();
    }

    void Update()
    {
    }

    void OnValidate()
    {
        foreach (DialogueLine line in dialogueLines)
            line.manager = this;
    }

    void DisplayDialogueLines()
    {
        var displayCoroutine = dialogueLines[lineIndex].GetDisplayCoroutine();

        typingCoroutine = StartCoroutine(displayCoroutine);
    }
}
