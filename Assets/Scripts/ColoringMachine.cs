using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoringMachine : MonoBehaviour
{
    private List<Colorable> targets = new List<Colorable>();
    [SerializeField] private Color currentColor = Color.white;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private ParticleSystem absorbParticles;
    [SerializeField] private MeshRenderer[] objectsToDisplayColor;

    [SerializeField] private AudioClip absorbSound;
    [SerializeField] private AudioClip colorSound;

    private AudioSource audioSource;
    private ParticleSystem[] allParticles;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        allParticles = new ParticleSystem[]{ particles, absorbParticles };
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
        audioSource.PlayOneShot(colorSound);
        particles.Play();
        foreach (Colorable colorable in targets)
            colorable.SetColor(currentColor);
    }

    public void SetColor(Color color)
    {
        color.a = 1;
        foreach(ParticleSystem system in allParticles)
        {
            ParticleSystem.MainModule main = system.main;
            main.startColor = color;
        }
        currentColor = color;
        foreach(MeshRenderer r in objectsToDisplayColor)
        {
            r.material.color = color;
            r.material.SetColor("_EmissionColor", color);
        }
    }

    public void AbsorbObject(Colorable colorable)
    {
        audioSource.PlayOneShot(absorbSound);
        SetColor(colorable.Color);
        Destroy(colorable.gameObject);
        absorbParticles.Play();
    }
}
