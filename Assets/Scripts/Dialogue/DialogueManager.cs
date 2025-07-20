using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Configuration")]
    public TextMeshProUGUI textMesh;
    public Color textColor;

    [Header("Dialogue")]
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    [HideInInspector]
    public UnityEvent lineDisplayStart;
    [HideInInspector]
    public UnityEvent lineDisplayStop;
    [HideInInspector]
    public UnityEvent linePrepared;
    [HideInInspector]
    public UnityEvent currentLineDiscarded;
    [HideInInspector]
    public UnityEvent anyInputPressed;

    private Coroutine dialogueCoroutine = null;
    private IEnumerator<DialogueLine> lineIterator;
    private DialogueLine currentLine;

    void Awake()
    {
        textMesh.color = textColor;
        textMesh.text = "";

        foreach (DialogueLine line in dialogueLines)
            line.Initialize(this);

        lineIterator = dialogueLines.GetEnumerator();
    }

    void Start()
    {
        LoadNextLine();
    }

    void Update()
    {
        if (currentLine == null)
            return ;

        if (currentLine.state == DialogueLineState.Typing)
            currentLine.Update();
        else if (Mouse.current.leftButton.wasPressedThisFrame)
            anyInputPressed.Invoke();
    }

    DialogueLine NextLine()
    {
        if (lineIterator.MoveNext())
            return lineIterator.Current;
        return null;
    }

    void SetupLineEvents()
    {
        currentLine?.startTrigger.AddListener(DisplayLine);
        currentLine?.stopTrigger.AddListener(DiscardLine);
        linePrepared.Invoke();
    }

    void LoadNextLine()
    {
        currentLine = NextLine();
        SetupLineEvents();
    }

    void DisplayLine()
    {
        dialogueCoroutine = StartCoroutine(
            RunDisplayCoroutine(
                currentLine.GetDisplayCoroutine()
            )
        );
    }

    void DiscardLine()
    {
        if (currentLine.state != DialogueLineState.Displayed)
            return ;

        textMesh.text = "";
        currentLine.startTrigger.RemoveListener(DisplayLine);
        currentLine.stopTrigger.RemoveListener(DiscardLine);
        LoadNextLine();
        currentLineDiscarded.Invoke();
    }

    IEnumerator RunDisplayCoroutine(IEnumerator displayCoroutine)
    {
        lineDisplayStart.Invoke();
        yield return displayCoroutine;
        lineDisplayStop.Invoke();
    }
}
