using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoringMachineAbsorber : MonoBehaviour
{
    private ColoringMachine machine;

    private void Awake()
    {
        machine = GetComponentInParent<ColoringMachine>();
        if (machine == null) Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Colorable colorable = other.GetComponent<Colorable>();
        if (colorable != null)
            machine.AbsorbObject(colorable);
    }
}
