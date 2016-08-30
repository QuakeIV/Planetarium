using UnityEngine;
using System.Collections;

public abstract class Planet : CelestialOrbiter
{
	public double density;
	public double atmoPressure;
	public double atmoRadius;
	public double debugVelocity;
	public double debugAltitude;
	public double debugGravity;

    void FixedUpdate()
    {
        UpdatePosition(Time.fixedDeltaTime);
        debugVelocity = velocity.magnitude;
        debugAltitude = position.magnitude;
        debugGravity = getParentGravity(position).magnitude;
    }

    public static Planet NewPlanet(CelestialObj parent)
    {
        if(Random.value > 0.0f)
        {
            return Terrestrial.Init(parent);
        }
        return GasGiant.Init(parent);
    }
    
    public abstract void UpdateAtmo(double pressure);

    public abstract double GetAtmoPressure(double altitude);
}
