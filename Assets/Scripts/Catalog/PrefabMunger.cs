using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class PrefabMunger : MonoBehaviour
{
    public void MungePhysics()
    {
        RemoveRotation(gameObject);
        if(gameObject.TryGetComponent<InteractableUnityEventWrapper>(out InteractableUnityEventWrapper interactableUnityEventWrapper))
        {
            interactableUnityEventWrapper.WhenHover.RemoveAllListeners();
            interactableUnityEventWrapper.WhenUnhover.RemoveAllListeners();
        }

        DisableColliderTriggers(gameObject);

        if(!TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            gameObject.AddComponent<Rigidbody>();
        }
        gameObject.AddComponent<VelocityBrake>();
        rb.isKinematic = false;
    }

    public void MungeInteraction(bool hideCanvas)
    {
        if(hideCanvas) HideCanvases(gameObject);
        RemoveInteractors(gameObject);
        RemoveRotation(gameObject);
        RemoveColliders(gameObject);
        if(hideCanvas) DisableChemicalEntity(gameObject);
        DisableTTSInteraction(gameObject);
    }

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

    private void DisableColliderTriggers(GameObject entity)
    {
        for(int i = 0; i < entity.transform.childCount; i++)
        {
            SphereCollider[] sphereColliders = entity.transform.GetChild(i).GetComponentsInChildren<SphereCollider>();
            foreach (Collider collider in sphereColliders)
            {
                collider.isTrigger = false;
            }
            CapsuleCollider[] capsuleColliders = entity.transform.GetChild(i).GetComponentsInChildren<CapsuleCollider>();
            foreach (Collider collider in capsuleColliders)
            {
                collider.isTrigger = false;
            }
        }
    }
}
