using UnityEngine;

public class WatchBehaviour : MonoBehaviour
{
    private const float _alphaActivated = 1f;
    private const float _alphaDeactivated = 0.1f;

    private bool _isActive = false;

    private MeshRenderer[] _meshRenderers;

    private void Start()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();

        EventManager.StartListening(EventType.PinchSelectorSelected, ToggleActive);
    }

    private void ToggleActive(System.Object obj)
    {
        _isActive = !_isActive;
        SetMaterialAlpha(_isActive ? _alphaActivated : _alphaDeactivated);
    }

    private void SetMaterialAlpha(float alpha)
    {
        foreach (MeshRenderer meshRenderer in _meshRenderers)
        {
            Material[] materials = meshRenderer.materials;
            foreach (Material material in materials)
            {
                Color color = material.GetColor("_BaseColor");
                color.a = alpha;
                material.SetColor("_BaseColor", color);
            }
            meshRenderer.materials = materials;
        }
    }


}
