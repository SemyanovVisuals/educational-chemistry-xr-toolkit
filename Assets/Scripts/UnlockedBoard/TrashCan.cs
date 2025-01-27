using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private GameObject _destroyMe;
    private bool _rotating = false;
    private void Update()
    {
        if(!_rotating) return;

        transform.Rotate(transform.up, -50f*Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.gameObject.name);
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
        if(other.gameObject.name.Contains("Bond"))
        {
            _destroyMe = null;
            _rotating = false;
            CancelInvoke(nameof(DestroyEntity));
        }
    }

    private void DestroyEntity()
    {
        if(_destroyMe == null) return;

        Destroy(_destroyMe);

        _rotating = false;
        _destroyMe = null;
    }
}
