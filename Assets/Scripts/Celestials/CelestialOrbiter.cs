using UnityEngine;
using System.Collections;

public abstract class CelestialOrbiter : CelestialObj
{
    //if you are an orbiting object, then you have a 'parent' that is pulling you
    public CelestialObj parent;

    //all planets and stars are meant to calculate their sphere of influence in the same way
    public override void UpdateSOI()
    {
        soi = Mathd.Sqrt(gravParam * 40000.0d);

        //set SOI radius
        //spherecollider is no longer going to be used for this
        /*SphereCollider sc = this.GetComponent<SphereCollider>();
        sc.radius = (float)soi;*/
    }

    //at this point in time, all planets and stars update their positions in the same way
    public void UpdatePosition(double deltaT)
    {
        Vector3d a1, a2, a3, a4;
        Vector3d v1, v2, v3, v4;
        Vector3d k1, k2, k3, k4;

        deltaT *= Universe.timeWarp;

        v1 = eulerVel(position, velocity, deltaT);
        k1 = v1 * deltaT;
        v2 = eulerVel(position + (k1 / 2.0d), ((v1 - velocity) / 2.0d) + velocity, deltaT / 2.0d);
        k2 = v2 * deltaT;
        v3 = eulerVel(position + (k2 / 2.0d), ((v2 - velocity) / 2.0d) + velocity, deltaT / 2.0d);
        k3 = v3 * deltaT;
        v4 = eulerVel(position + k3, v3, deltaT);
        k4 = v4 * deltaT;

        Vector3d newPos = position + ((k4 + 2.0d * k3 + 2.0d * k2 + k1) / 6.0d);

        a1 = getParentGravity(position);
        k1 = a1 * deltaT;
        a2 = getParentGravity(position + k1 * deltaT / 2);
        k2 = a2 * deltaT / 2 + a1 * deltaT / 2;
        a3 = getParentGravity(position + k2 * deltaT / 2);
        k3 = a3 * deltaT / 2 + a2 * deltaT / 2;
        a4 = getParentGravity(position + k3 * deltaT);
        k4 = a4 * deltaT;

        velocity = velocity + (k1 + 2.0d * k2 + 2.0d * k3 + k4) / 6.0d;

        position = newPos;
        transform.localPosition = (Vector3)position;
    }

    //meant to be used in context of some 'special event' where a high gravity object is upsetting various bodies trajectories
    //this should probably function completely differently from this, with planets recieving pull from other planets if they enter eachothers SOI's as a result of the acceleration
    public void UpdatePosition(double deltaT, Vector3d addedAccel)
    {
        Vector3d acceleration = getParentGravity(position);
        acceleration += addedAccel;
        velocity += acceleration * deltaT;
        position += velocity * deltaT;

        transform.localPosition = (Vector3)position;
    }
    

    //invoke this to add a pretty trail to visualize movement
    public void addTrail()
    {
        addTrail(new Color(255.0f, 0.0f, 0.0f));
    }

    public void addTrail(Color clr)
    {
        if(gameObject.GetComponent<TrailRenderer>() == null)
        {
            TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
            trail.time = 3600;
            trail.startWidth = 20;
            trail.endWidth = 10;
            Material trailcolor = trail.material;
            trailcolor.shader = Shader.Find("Unlit/Color");
            trailcolor.SetColor("_Color", clr);
            trail.material = trailcolor;
        }
    }

    //helpers
    protected Vector3d getParentGravity(Vector3d mPos)
    {
        return (-1.0d * (mPos / mPos.magnitude)) * ((parent.gravParam) / (mPos.magnitude * mPos.magnitude));
    }

    protected Vector3d eulerVel(Vector3d pos, Vector3d vel, double deltaT)
    {
        return vel + (getParentGravity(pos) * deltaT);
    }
}
