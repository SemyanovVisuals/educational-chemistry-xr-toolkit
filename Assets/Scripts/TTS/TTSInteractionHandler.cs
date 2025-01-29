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

    }

}
