using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class Hologram : MonoBehaviour
{
    private Color _startColor;
    private Color _endColor;
    private MaterialPropertyBlock _materialPropertyBlock;
    private List<Mesh> _meshes = new List<Mesh>();
    private MeshRenderer[] _meshRenderers;
    private RenderParams _renderParams;

    private void Start()
    {
        StartHologramify();
        StartHideCanvases();
        StartRemoveInteractors();
        // transform.up = transform.parent.forward;
    }
    
    private void StartHologramify()
    {
        _startColor = Color.Lerp(Color.red, Color.yellow, Random.Range(0.0f, 1.0f));
        _endColor = Color.Lerp(Color.blue, Color.green, Random.Range(0.0f, 1.0f));
        
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in _meshRenderers)
        {
            _meshes.Add(meshRenderer.GetComponent<MeshFilter>().sharedMesh);
            meshRenderer.enabled = false;
        }

        Material material = Resources.Load<Material>("Materials/Hologram");
        _materialPropertyBlock = new MaterialPropertyBlock();
        _materialPropertyBlock.SetColor("_BaseColor", _startColor);

        _renderParams = new RenderParams(material)
        {
            matProps = _materialPropertyBlock
        };
    }

    private void StartHideCanvases()
    {
        Canvas[] canvases = GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    private void StartRemoveInteractors()
    {
        PointableElement[] pointableElements = GetComponentsInChildren<PointableElement>();
        foreach (PointableElement pointableElement in pointableElements)
        {
            pointableElement.enabled = false;
        }
        
        HandGrabInteractable[] handGrabInteractables = GetComponentsInChildren<HandGrabInteractable>();
        foreach (HandGrabInteractable handGrabInteractable in handGrabInteractables)
        {
            handGrabInteractable.enabled = false;
        }

        GrabInteractable[] grabInteractables = GetComponentsInChildren<GrabInteractable>();
        foreach (GrabInteractable grabInteractable in grabInteractables)
        {
            grabInteractable.enabled = false;
        }

        RotationObjects[] rotationObjects = GetComponentsInChildren<RotationObjects>();
        foreach (RotationObjects rotationObject in rotationObjects)
        {
            rotationObject.enabled = false;
        }
    }

    private void Update()
    {
        // transform.up = transform.parent.forward;
        transform.Rotate(transform.forward, -50.0f*Time.deltaTime);
        
        // just return randomly 2 out of 3 to create flickering effect
        if (UnityEngine.Random.Range(0, 100) < 10) return;

        // set render color to random color between start and end color
        Color materialColor = Color.Lerp(_startColor, _endColor, Mathf.PingPong(Time.time, 1));
        _materialPropertyBlock.SetColor("_BaseColor", materialColor);
        _renderParams.matProps = _materialPropertyBlock;
        
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(
                _meshRenderers[i].transform.position,
                _meshRenderers[i].transform.rotation,
                _meshRenderers[i].transform.lossyScale
            );
            Graphics.RenderMesh(_renderParams, _meshes[i], 0, matrix);
        }
    }
    
}
