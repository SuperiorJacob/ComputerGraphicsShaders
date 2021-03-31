using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public Transform[] feet;
    public VisualFXSystem.VisualFX fx;

    public void Step(int foot)
    {
        fx.Begin(feet[foot - 1]);
    }
}
