using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class GrabReleaseDetector : MonoBehaviour
{
    private HandGrabInteractable handGrabInteractable;

    private void Start()
    {
        handGrabInteractable = GetComponent<HandGrabInteractable>();
        if (handGrabInteractable != null)
        {
            handGrabInteractable.WhenPointerEventRaised += OnGrabEvent;
        }
    }

    private void OnGrabEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Unselect)
        {
            Debug.Log("Molecule released!");
            OnRelease();
        }
    }

    private void OnRelease()
    {
        // Add logic for what happens when released
        Debug.Log("Performing actions post-release...");
    }
}