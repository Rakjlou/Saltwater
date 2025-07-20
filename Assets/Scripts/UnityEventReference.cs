using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventReference
{
    public Component targetComponent;
    public string eventFieldName;

    private UnityEvent unityEvent = null;

    public void Initialize()
    {
        unityEvent = targetComponent
            .GetType()
            .GetField(eventFieldName)
            ?.GetValue(targetComponent) as UnityEvent
        ;
    }

    public UnityEvent GetEvent()
    {
        if (unityEvent != null)
            return unityEvent;
        else if (targetComponent == null)
            return null;

        Initialize();
        return unityEvent;
    }

    public void AddListener(UnityAction callback)
    {
        GetEvent()?.AddListener(callback);
    }

    public void RemoveListener(UnityAction callback)
    {
        GetEvent()?.RemoveListener(callback);
    }
}
