using UnityEngine;

public class PinchSelectorBehaviour : MonoBehaviour
{
    public void OnSelected()
    {
        EventManager.TriggerEvent(EventType.PinchSelectorSelected);
    }

    public void OnUnselected()
    {
        EventManager.TriggerEvent(EventType.PinchSelectorUnselected);
    }
}
