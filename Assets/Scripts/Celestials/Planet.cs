using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour 
{
	public double soi;
    public double timeWarp;
	public double density;
	public double radius;
    public double gravParam;
	public double surfaceGravity;
	public double atmoPressure;
	public double atmoRadius;
	public double mass;
	public double debugVelocity;
	public double debugAltitude;
	public double debugGravity;
	public Vector3d velocity;
	public Vector3d position;
	public Planet parent;
	public PlanetIndicator model;

    void Awake () 
	{
	}

	void Start ()
	{
		surfaceGravity = gravParam * 1000.0f / (radius * radius);
        timeWarp = 60.0d;
	}

    void FixedUpdate()
    {
        UpdatePosition(Time.fixedDeltaTime);
        debugVelocity = velocity.magnitude;
        debugAltitude = position.magnitude;
        debugGravity = computeAccel(position).magnitude;
    }

	public void UpdatePosition(double deltaT)
    {
        Vector3d a1, a2, a3, a4;
        Vector3d v1, v2, v3, v4;
        Vector3d k1, k2, k3, k4;

        deltaT *= timeWarp;
        
        v1 = derpVel(position, velocity, deltaT);
        k1 = v1 * deltaT;
        v2 = derpVel(position + (k1 / 2.0d), ((v1 - velocity) / 2.0d) + velocity, deltaT / 2.0d);
        k2 = v2 * deltaT;
        v3 = derpVel(position + (k2 / 2.0d), ((v2 - velocity) / 2.0d) + velocity, deltaT / 2.0d);
        k3 = v3 * deltaT;
        v4 = derpVel(position + k3, v3, deltaT);
        k4 = v4 * deltaT;

        Vector3d newPos = position + ((k4 + 2.0d * k3 + 2.0d * k2 + k1) / 6.0d);

        a1 = computeAccel(position);
        k1 = a1 * deltaT;
        a2 = computeAccel(position + k1 * deltaT / 2);
        k2 = a2 * deltaT / 2 + a1 * deltaT / 2;
        a3 = computeAccel(position + k2 * deltaT / 2);
        k3 = a3 * deltaT / 2 + a2 * deltaT / 2;
        a4 = computeAccel(position + k3 * deltaT);
        k4 = a4 * deltaT;

        velocity = velocity + (k1 + 2.0d * k2 + 2.0d * k3 + k4) / 6.0d;
        
        /*velocity += computeAccel(position) * deltaT;
		//try to implement runge-kutta at some point
		position += velocity * deltaT;*/

        position = newPos;
        transform.localPosition = (Vector3)position;
    }	

	public void UpdatePosition(double deltaT, Vector3d addedAccel)
	{
        Vector3d acceleration = computeAccel(position);
		acceleration += addedAccel;
		velocity += acceleration * deltaT;
		//try to implement runge-kutta at some point
		position += velocity * deltaT;
		
		transform.localPosition = (Vector3)position;
	}

	public void UpdateSOI()
	{
		soi = Mathd.Sqrt(gravParam * 40000.0d);

		//set SOI radius
		SphereCollider sc = this.GetComponent<SphereCollider>();
		sc.radius = (float)soi;
	}

	public void UpdateVolume ()
	{
		radius = Mathd.Pow((mass * 3.0d)/(4.0d * Mathf.PI * density), 1.0d / 3.0d);
		model.UpdateRadius(radius);
	}

	public void UpdateAtmo(double pressure)
	{
		atmoPressure = pressure;
		atmoRadius = 1.4f * radius;
		Debug.Log("UpdateAtmo Run with " + pressure);
	}

	public double GetAtmoPressure(double altitude)
	{
		return -atmoPressure * altitude / atmoRadius + atmoPressure;
	}

    public Vector3d computeAccel(Vector3d mPos)
    {
        return (-1.0d * (mPos / mPos.magnitude)) * ((parent.gravParam) / (mPos.magnitude * mPos.magnitude));
    }

    private Vector3d derpVel(Vector3d pos, Vector3d vel, double deltaT)
    {
        return vel + (computeAccel(pos) * deltaT);
    }

    /*
    private Vector3d rungeVel(Vector3d mPos, Vector3d mVel, double deltaT)
    {
        Vector3d acceleration = computeAccel(mPos);
        Vector3d k1, k2, k3, k4;
        k1 = acceleration * deltaT / 2.0d;
        k2 = computeAccel(mPos + (k1 + mVel) * deltaT / 2.0d) * deltaT / 2.0d;
        k3 = computeAccel(mPos + (k2 + mVel) * deltaT / 2.0d) * deltaT / 2.0d;
    }*/

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
