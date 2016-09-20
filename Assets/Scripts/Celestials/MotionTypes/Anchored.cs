using UnityEngine;
using System.Collections;
using System;

public class Anchored : OrbitType
{
    public override void UpdatePosition()
    {
        //do nothing, stay put...
    }

    public override double GetSemiMajorAxis()
    {
        return 0.0d;
    }
}
