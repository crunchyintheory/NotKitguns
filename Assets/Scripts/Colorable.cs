using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorable : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material baseMaterial;
    [SerializeField] private Color _currentColor;

    public Color Color { get { return _currentColor; } private set { _currentColor = value; } }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        baseMaterial = meshRenderer.material;
        Color = baseMaterial.color;
    }

    public void SetColor(Color color)
    {
        Color = color;
        meshRenderer.material.color = color;
    }

    public void ResetColor()
    {
        Color = baseMaterial.color;
        meshRenderer.material.color = baseMaterial.color;
    }
}
