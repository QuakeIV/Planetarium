using UnityEngine;
using System.Collections;
using System;

public class TerrestrialPlanet : CelestialObj
{
    public override void SetupOrbit()
    {
        orbit = new UVFKinetic(this, position, velocity);
    }

    /*public override void UpdateAtmo()
    {
        //iffy at best, may be best in general to cut this idea
        /*
        surfaceAtmoPressure = pressure;
        atmoRadius = 1.4f * radius;
        Debug.Log("UpdateAtmo Run with " + pressure);
    }

    public override double GetAtmoPressure(double altitude)
    {
        return -surfaceAtmoPressure * altitude / atmoRadius + surfaceAtmoPressure;
    }*/
}
