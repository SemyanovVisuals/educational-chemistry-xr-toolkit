using System.Runtime.CompilerServices;
using Oculus.Interaction.Editor.Generated;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    [SerializeField] private GameObject _bottom;

    private HandGrabInteractable _handGrabInteractable;
    private GameObject _destroyMe;

    private bool _destroying = false;
    private bool _rotating = false;
    private void Update()
    {
        UpdateRotation();
        UpdateDestroy();
    }

    private void UpdateRotation()
    {
        if(!_rotating) return;

        transform.Rotate(transform.up, -150f*Time.deltaTime);
    }

    private void UpdateDestroy()
    {
        if(!_destroying) return;
        // if(_handGrabInteractable && _handGrabInteractable.SelectingInteractors.Count > 0) return;

        float distance = Vector3.Distance(_destroyMe.transform.position, _bottom.transform.position);
        if(distance < 0.01f)
        {
            Destroy(_destroyMe);

            _destroyMe = null;
            _handGrabInteractable = null;
            _destroying = false;
            _rotating = false;
        }
        else
        {
            _destroyMe.transform.localScale = Vector3.Lerp(_destroyMe.transform.localScale, Vector3.zero, Time.deltaTime*10f);
            _destroyMe.transform.position = Vector3.MoveTowards(_destroyMe.transform.position, _bottom.transform.position, Time.deltaTime*0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.gameObject.name);
        if(_destroying) return;
        if(other.gameObject.name.Contains("Bond"))
        {
            _destroyMe = other.gameObject.transform.parent.parent.gameObject;
            _rotating = true;
            Invoke(nameof(DestroyEntity), 3f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited collision with: " + other.gameObject.name);
        if(_destroying) return;
        if(other.gameObject.name.Contains("Bond"))
        {
            _destroyMe = null;
            _destroying = false;
            _rotating = false;
            CancelInvoke(nameof(DestroyEntity));
        }
    }
    
    private void DestroyEntity()
    {
        if(_destroyMe == null) return;

        _handGrabInteractable = _destroyMe.GetComponent<HandGrabInteractable>();
        _destroying = true;
    }
}
