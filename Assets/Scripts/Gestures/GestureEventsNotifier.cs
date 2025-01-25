using UnityEngine;

public class GestureEventsNotifier : MonoBehaviour
{
    [SerializeField] private EventType _activatedEvent;
    [SerializeField] private EventType _deactivatedEvent;

    public void OnActivated()
    {
        EventManager.TriggerEvent(_activatedEvent);
    }

    public void OnDeactivated()
    {
        EventManager.TriggerEvent(_deactivatedEvent);
    }
}
