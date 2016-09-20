using UnityEngine;
using System.Collections;
using System;

public class Star : CelestialObj
{
    public override void SetupOrbit()
    {
        orbit = new Anchored();
    }
}
