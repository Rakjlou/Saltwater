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

    public UnityEvent lineDisplayStart;
    public UnityEvent lineDisplayStop;
    public UnityEvent linePrepared;
    public UnityEvent currentLineDiscarded;
    public UnityEvent anyInputPressed;

    private Coroutine dialogueCoroutine = null;
    private IEnumerator<DialogueLine> linesIterator;
    private DialogueLine newCurrentLine;

    void Awake()
    {
        textMesh.color = textColor;
        textMesh.text = "";

        foreach (DialogueLine line in dialogueLines)
            line.Initialize(this);

        linesIterator = dialogueLines.GetEnumerator();
    }

    void Start()
    {
        LoadNextLine();
    }

    void Update()
    {
        if (newCurrentLine == null)
            return ;

        if (newCurrentLine.state == DialogueLineState.Typing)
            newCurrentLine.TypingUpdate();
        else if (Mouse.current.leftButton.wasPressedThisFrame)
            anyInputPressed.Invoke();
    }

    DialogueLine NextLine()
    {
        if (linesIterator.MoveNext())
            return linesIterator.Current;
        return null;
    }

    void PrepareLine()
    {
        newCurrentLine?.startTrigger.AddListener(DisplayLine);
        newCurrentLine?.stopTrigger.AddListener(DiscardLine);
        linePrepared.Invoke();
    }

    void LoadNextLine()
    {
        newCurrentLine = NextLine();
        PrepareLine();
    }

    void DisplayLine()
    {
        dialogueCoroutine = StartCoroutine(
            MonitorDisplay(
                newCurrentLine.GetDisplayCoroutine()
            )
        );
    }

    void DiscardLine()
    {
        if (newCurrentLine.state != DialogueLineState.Displayed)
            return ;

        textMesh.text = "";
        newCurrentLine.startTrigger.RemoveListener(DisplayLine);
        newCurrentLine.stopTrigger.RemoveListener(DiscardLine);
        LoadNextLine();
        currentLineDiscarded.Invoke();
    }

    IEnumerator MonitorDisplay(IEnumerator displayCoroutine)
    {
        lineDisplayStart.Invoke();
        yield return displayCoroutine;
        lineDisplayStop.Invoke();
    }
}
