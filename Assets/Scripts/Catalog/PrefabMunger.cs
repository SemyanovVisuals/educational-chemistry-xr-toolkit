using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class PrefabMunger : MonoBehaviour
{
    protected void HideCanvases(GameObject entity)
    {
        Canvas[] canvases = entity.GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    protected void RemoveInteractors(GameObject entity)
    {
        PointableElement[] pointableElements = entity.GetComponentsInChildren<PointableElement>();
        foreach (PointableElement pointableElement in pointableElements)
        {
            pointableElement.enabled = false;
        }
        
        HandGrabInteractable[] handGrabInteractables = entity.GetComponentsInChildren<HandGrabInteractable>();
        foreach (HandGrabInteractable handGrabInteractable in handGrabInteractables)
        {
            handGrabInteractable.enabled = false;
        }

        GrabInteractable[] grabInteractables = entity.GetComponentsInChildren<GrabInteractable>();
        foreach (GrabInteractable grabInteractable in grabInteractables)
        {
            grabInteractable.enabled = false;
        }
    }

    protected void RemoveColliders(GameObject entity)
    {
        Collider[] colliders = entity.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    protected void RemoveRotation(GameObject entity)
    {
        RotationObjects[] rotationObjects = entity.GetComponentsInChildren<RotationObjects>();
        foreach (RotationObjects rotationObject in rotationObjects)
        {
            rotationObject.enabled = false;
        }
    }

    protected void DisableChemicalEntity(GameObject entity)
    {

        ChemicalEntity chemicalEntity = entity.GetComponentInChildren<ChemicalEntity>();
        chemicalEntity.enabled = false;
    }

    protected void DisableTTSInteraction(GameObject entity)
    {
        if(entity.TryGetComponent<InteractableUnityEventWrapper>(out InteractableUnityEventWrapper interactableUnityEventWrapper))
        {
            interactableUnityEventWrapper.WhenHover.RemoveAllListeners();
            interactableUnityEventWrapper.WhenUnhover.RemoveAllListeners();
        }
    }
}
