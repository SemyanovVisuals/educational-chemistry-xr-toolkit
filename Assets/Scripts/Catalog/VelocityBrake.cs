using Oculus.Interaction.HandGrab;
using Unity.VisualScripting;
using UnityEngine;

public class VelocityBrake : MonoBehaviour
{
    private float _brakeForce = 0.5f;
    private float _maxLinearVelocity = 0.1f;
    private float _maxAngularVelocity = 0.1f;

    private HandGrabInteractable _handGrabInteractable;
    private Rigidbody _rigidbody;
    private Summonable _summonable;

    private void Start()
    {
        _handGrabInteractable = GetComponent<HandGrabInteractable>();

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.maxLinearVelocity = _maxLinearVelocity;
        _rigidbody.maxAngularVelocity = _maxAngularVelocity;

        _summonable = GetComponent<Summonable>();
    }

    private void Update()
    {
        if(_handGrabInteractable.SelectingInteractors.Count > 0) return;
        if(_summonable && _summonable.IsSummoning()) return;

        _rigidbody.linearVelocity = Vector3.Lerp(_rigidbody.linearVelocity, Vector3.zero, Time.deltaTime*_brakeForce);
        _rigidbody.angularVelocity = Vector3.Lerp(_rigidbody.angularVelocity, Vector3.zero, Time.deltaTime*_brakeForce);
    }
}
