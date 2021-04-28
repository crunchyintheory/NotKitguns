using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    [SerializeField] private ColoringMachine target;

    public void Interact()
    {
        target.ColorTargets();
    }
}
