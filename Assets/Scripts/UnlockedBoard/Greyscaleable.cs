using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class Greyscaleable : MonoBehaviour
{
    private MeshRenderer[] _meshRenderers;
    private TextMeshProUGUI[] _textMeshes;
    private List<Color[]> _meshColors = new List<Color[]>();
    private List<Color> _textMeshColors = new List<Color>();
    private bool _isGreyscale = false;

    void Start()
    {
        StartGetColorsForCanvasText();
        StartGetMaterialColorsForMeshRenderers();
        StartGrayscaleOrColor();
    }

    private void StartGetColorsForCanvasText()
    {
        _textMeshes = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textMesh in _textMeshes)
        {
            _textMeshColors.Add(textMesh.color);
        }
    }

    private void StartGetMaterialColorsForMeshRenderers()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in _meshRenderers)
        {
            _meshColors.Add(StartGetMaterialColors(meshRenderer));
        }
    }

    private Color[] StartGetMaterialColors(MeshRenderer meshRenderer)
    {
        Material[] materials = meshRenderer.materials;
        Color[] colors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            colors[i] = materials[i].color;
        }
        return colors;
    }

    private void StartGrayscaleOrColor()
    {
        if (_isGreyscale)
        {
            MakeGrayscale();
        }
        else
        {
            MakeColorful();
        }
    }

    public void MakeGrayscale()
    {
        _isGreyscale = true;
        if(_meshRenderers == null) return;
        foreach (MeshRenderer meshRenderer in _meshRenderers)
        {
            Material[] materials = meshRenderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = Color.gray;
            }
            meshRenderer.materials = materials;
        }

        foreach (TextMeshProUGUI textMesh in _textMeshes)
        {
            textMesh.color = Color.gray;
        }
    }

    public void MakeColorful()
    {
        _isGreyscale = false;
        if(_meshRenderers == null) return;

        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            Material[] materials = _meshRenderers[i].materials;
            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].color = _meshColors[i][j];
            }
            _meshRenderers[i].materials = materials;
        }

        for (int i = 0; i < _textMeshes.Length; i++)
        {
            _textMeshes[i].color = _textMeshColors[i];
        }
    }

}
