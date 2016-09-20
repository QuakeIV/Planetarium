using UnityEngine;
using System.Collections;

public abstract class CelestialObj : MonoBehaviour
{
    public CelestialObj parent;
    public CelestialIndicator model;
    public OrbitType orbit;
    public Vector3d position;
    public Vector3d velocity;
    public double density;
    public double surfaceAtmoPressure;
    public double atmoRadius;
    public double soi;
    public double radius;
    public double gravParam;
    public double mass;

    //movers are responsible for updating these values
    public double debugVelocity;
    public double debugAltitude;
    public double debugRecievedGravity;
    public double debugSurfaceGravity;

    public void FixedUpdate()
    {
        orbit.UpdatePosition();
    }

    /*
    meant to set up the planets trajecotry
    */
    public abstract void SetupOrbit();

    /*responsible for maintaining 
    SOI
    radius
    notifying model that radius has changed
    gravParam
    atmospheric parameters
    */
    public void UpdateParameters()
    {
        UpdateRadius(); //UpdateRadius currently notifies model of the change in status
        gravParam = Universe.G * mass;

        UpdateSOI();
    }

    public void UpdateSOI()
    {
        if(mass == 0.0d)
        {
            throw new System.Exception("Object must be properly set up before invoking UpdateSOI");
        }
        if(parent == null)
        {
            soi = double.PositiveInfinity;
        }
        else
        {
            soi = orbit.GetSemiMajorAxis() * Mathd.Pow(parent.mass / mass, 2.0d/5.0d);
        }
    }

    public void UpdateRadius()
    {
        radius = Mathd.Pow((mass * 3.0d) / (4.0d * Mathd.PI * density), 1.0d / 3.0d);
        model.UpdateRadius(radius);
    }
}
