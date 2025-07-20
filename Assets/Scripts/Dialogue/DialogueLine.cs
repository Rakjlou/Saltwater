using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[System.Serializable]
public class DialogueLine
{
    [Header("Settings")]
    [Tooltip("Delay before this line can start after being triggered")]
    public float startDelay = 0f;

    [Tooltip("Can the player skip the text appearing animation ?")]
    public bool animationSkippable = true;

    [Tooltip("The actual line content")]
    [TextArea(3, 5)]
    public string text;

    [Header("Triggers")]
    public UnityEventReference startTrigger;
    public UnityEventReference stopTrigger;

    public DialogueManager manager { get; set; }
    public DialogueLineState state { get; private set; }

    private bool skipRequested = false;

    public IEnumerator GetDisplayCoroutine()
    {
        // TODO: Have multiple animation types, do a switch case here
        return CreateTypingAnimationCoroutine();
    }

    public void Initialize(DialogueManager parentManager)
    {
        manager = parentManager;
        state = DialogueLineState.Idle;

        startTrigger.Initialize();
        stopTrigger.Initialize();
    }

    public void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            skipRequested = true;
    }

    IEnumerator CreateTypingAnimationCoroutine()
    {
        yield return new WaitForSeconds(startDelay);

        state = DialogueLineState.Typing;
        manager.textMesh.text = "";

        foreach (char c in text)
        {
            manager.textMesh.text += c;

            if (animationSkippable && skipRequested)
                continue ;

            yield return new WaitForSeconds(0.05f);
        }

        state = DialogueLineState.Displayed;
    }
}
