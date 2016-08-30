using UnityEngine;
using System.Collections;
using System;

public class GasGiant : Planet
{
    public static Planet Init(CelestialObj parent)
    {
        return null;
    }

    public override void UpdateAtmo(double pressure)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRadius()
    {
        throw new NotImplementedException();
    }

    public override double GetAtmoPressure(double altitude)
    {
        throw new NotImplementedException();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
