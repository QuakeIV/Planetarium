using UnityEngine;
using System.Collections;

public abstract class CelestialObj : MonoBehaviour
{
    public Universe.CelestialType type;
    public double soi;
    public double radius;
    public double gravParam;
    public double mass;
    public Vector3d position;
    public Vector3d velocity;
    public CelestialIndicator model;

    //mandate that everyone do something to set up their SOI
    public abstract void UpdateSOI();

    public abstract void UpdateRadius();

    public abstract void Init(CelestialObj parent);
    
    //SOI system is also meant to be used to decide which object spacecraft are child-ed to for purposes of patched conic 
    //no longer using only the transform to manage position and velocity, this will need modification to bring back into service
    /*
    //if something enters the collision field, make it a child of the planet and start gravitying on it
    void OnTriggerEnter(Collider other)
    {
        print("honk");
        if (!(this.transform.parent == other))
        {
            other.gameObject.transform.parent = this.transform;
        }
    }

    //if it exits the collision field, pass the torch on to the parent object
    void OnTriggerExit(Collider other)
    {		
        print("heunk");
        if (!(this.transform.parent == other))
        {
            other.gameObject.transform.parent = this.transform;
        }
    }*/
}
