using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoringMachine : MonoBehaviour
{
    private List<Colorable> targets = new List<Colorable>();
    [SerializeField] private Color currentColor = Color.white;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private MeshRenderer[] objectsToDisplayColor;

    private IEnumerator TestCoroutine()
    {
        yield return new WaitForSeconds(2);
        ColorTargets();
    }

    private void Awake()
    {
        StartCoroutine(TestCoroutine());
        currentColor = Color.green;
        SetColor(currentColor);
    }

    public void AddTarget(Colorable target)
    {
        targets.Add(target);
    }

    public void RemoveTarget(Colorable target)
    {
        targets.Remove(target);
    }
    
    public void ClearTargets()
    {
        targets.Clear();
    }

    public void ColorTargets()
    {
        particles.Play();
        foreach (Colorable colorable in targets)
            colorable.SetColor(currentColor);
    }

    public void SetColor(Color color)
    {
        ParticleSystem.MainModule main = particles.main;
        main.startColor = color;
        currentColor = color;
        foreach(MeshRenderer r in objectsToDisplayColor)
        {
            r.material.color = color;
            r.material.SetColor("_EmissionColor", color);
        }
    }

    public void AbsorbObject(Colorable colorable)
    {
        SetColor(colorable.Color);
        Destroy(colorable.gameObject);
    }
}
