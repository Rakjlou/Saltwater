using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventReference
{
    public Component targetComponent;
    public string eventFieldName;

    private UnityEvent targetEvent = null;

    public void Initialize()
    {
        targetEvent = targetComponent
            .GetType()
            .GetField(eventFieldName)
            ?.GetValue(targetComponent) as UnityEvent
        ;
    }

    public UnityEvent GetEvent()
    {
        if (targetEvent != null)
            return targetEvent;
        else if (targetComponent == null)
            return null;

        Initialize();
        return targetEvent;
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
