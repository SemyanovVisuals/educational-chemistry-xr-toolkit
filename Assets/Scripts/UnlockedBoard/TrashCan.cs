using System.Runtime.CompilerServices;
using Oculus.Interaction.Editor.Generated;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    [SerializeField] private AudioClip _destroySound;
    [SerializeField] private GameObject _bottom;

    private AudioSource _audioSource;
    private GameObject _destroyMe;
    private bool _destroying = false;
    private bool _rotating = false;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

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

        float distance = Vector3.Distance(_destroyMe.transform.position, _bottom.transform.position);
        if(distance < 0.01f)
        {
            Destroy(_destroyMe);

            _destroyMe = null;
            _destroying = false;
            _rotating = false;
            _audioSource.Stop();
        }
        else
        {
            _destroyMe.transform.localScale = Vector3.Lerp(_destroyMe.transform.localScale, Vector3.zero, Time.deltaTime*10f);
            _destroyMe.transform.position = Vector3.MoveTowards(_destroyMe.transform.position, _bottom.transform.position, Time.deltaTime*0.5f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(_destroying) return;

        ChemicalEntity chemicalEntity = other.gameObject.GetComponentInParent<ChemicalEntity>();
        if(chemicalEntity == null) return;
        _rotating = true;

        if(!_audioSource.isPlaying) _audioSource.Play();
        HandGrabInteractable handGrabInteractable = chemicalEntity.GetComponent<HandGrabInteractable>();
        if(handGrabInteractable == null || handGrabInteractable.SelectingInteractors.Count > 0) return;

        _destroyMe = chemicalEntity.gameObject;
        _destroying = true;
        _audioSource.PlayOneShot(_destroySound);
    }

    private void OnTriggerExit(Collider other)
    {
        if(_destroying) return;

        ChemicalEntity chemicalEntity = other.gameObject.GetComponentInParent<ChemicalEntity>();
        if(chemicalEntity != null && !_destroying)
        {
            _rotating = false;
            _audioSource.Stop();
        }
    }
}
