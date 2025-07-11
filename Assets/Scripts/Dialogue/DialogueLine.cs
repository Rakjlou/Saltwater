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

    [Header("Text")]
    [Tooltip("The actual line content")]
    [TextArea(3, 5)]
    public string text;


    public DialogueManager manager { get; set; }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public IEnumerator GetDisplayCoroutine()
    {
        // TODO: Have multiple animation types, do a switch case here
        return GetTypingCoroutine();
    }

    IEnumerator GetTypingCoroutine()
    {
        // TODO: Separation of concerns potential issue :
        // - Delay
        // - Skip animation
        // - Display

        yield return new WaitForSeconds(startDelay);

        var skipAction = new InputAction();
        var skipRequested = false;

        skipAction.AddBinding("<Mouse>/leftButton");
        skipAction.performed += ctx => {
            manager.textMesh.text = text;
            skipRequested = true;
        };

        if (animationSkippable)
            skipAction.Enable();

        manager.textMesh.text = "";

        foreach (char c in text)
        {
            if (skipRequested)
                break ;

            manager.textMesh.text += c;
            yield return new WaitForSeconds(0.05f);
        }

        skipAction.Disable();
        skipAction.Dispose();
    }
}
