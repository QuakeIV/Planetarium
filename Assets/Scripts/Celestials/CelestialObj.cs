using UnityEngine;
using System.Collections;

public abstract class CelestialObj : MonoBehaviour
{
    public static double timeWarp;
    public double soi;
    public double radius;
    public double gravParam;
    public double mass;
    public Vector3d velocity;
    public Vector3d position;
    public PlanetIndicator model;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
