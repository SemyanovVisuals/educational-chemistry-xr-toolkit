using Oculus.Interaction;
using UnityEngine;

public class TTSInteractionHandler : MonoBehaviour
{

    public InteractableUnityEventWrapper interactableUnityEventWrapper;
    public TTSHandler ttsHandler;

    private void Awake()
    {
        if (interactableUnityEventWrapper == null)
        {
            interactableUnityEventWrapper = GetComponent<InteractableUnityEventWrapper>();
        }

        if (ttsHandler == null)
        {
            ttsHandler = FindFirstObjectByType<TTSHandler>();
        }

        // Subscribe to the Unity events
        interactableUnityEventWrapper.WhenHover.AddListener(OnHover);
        interactableUnityEventWrapper.WhenUnhover.AddListener(OnUnhover);
        interactableUnityEventWrapper.WhenSelect.AddListener(OnSelect);
        interactableUnityEventWrapper.WhenUnselect.AddListener(OnUnselect);
    }

    
    private void OnHover()
    {
        ttsHandler.Speak(gameObject);
    }

    private void OnUnhover()
    {
        // Handle unhover event
        ttsHandler.StopSpeaking(gameObject);
    }

    private void OnSelect()
    {
        // Handle select event
    }

    private void OnUnselect()
    {
        // Handle unselect event
    }

}
