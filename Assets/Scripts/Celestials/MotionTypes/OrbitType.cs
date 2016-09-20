using UnityEngine;
using System.Collections;

/*
responsible for updating debug values within their parent objects
*/
public abstract class OrbitType
{
    public CelestialObj obj;
    public abstract void UpdatePosition();

    /*
    orbital parameters useful outside of the mover (list will likely be expanded)
    these are expected to return accurate information in the presence of changing conditions
    */
    public abstract double GetSemiMajorAxis();
}
