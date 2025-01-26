using UnityEngine;

public class GestureEventsNotifier : MonoBehaviour
{
    [SerializeField] private EventType _activatedEvent;
    [SerializeField] private EventType _deactivatedEvent;

    public void OnActivated(Object obj)
    {
        EventManager.TriggerEvent(_activatedEvent, obj);
    }

    public void OnDeactivated(Object obj)
    {
        EventManager.TriggerEvent(_deactivatedEvent, obj);
    }
}
