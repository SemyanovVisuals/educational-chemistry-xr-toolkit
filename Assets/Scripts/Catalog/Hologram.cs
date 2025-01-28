using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class Hologram : MonoBehaviour
{    
    private void Update()
    {
        UpdateRotate();
    }

    private void UpdateRotate()
    {
        transform.Rotate(0, 0, -50.0f * Time.deltaTime);
    }
}
