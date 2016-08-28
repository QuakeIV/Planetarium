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

    public override void Init(CelestialObj parent)
    {
        base.Init(parent);
    }

    void FixedUpdate()
    {
        UpdatePosition(Time.fixedDeltaTime);
        debugVelocity = velocity.magnitude;
        debugAltitude = position.magnitude;
        debugGravity = getParentGravity(position).magnitude;
    }
    
    public abstract void UpdateAtmo(double pressure);

    public abstract double GetAtmoPressure(double altitude);
}
