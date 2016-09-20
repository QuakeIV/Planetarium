using UnityEngine;
using System;

public class RungeKinetic : OrbitType
{
    public override void UpdatePosition()
    {
        if (obj.parent == null)
        {
            throw new NullReferenceException("Movement type invalid for celestial object without parent");
        }

        //begin debug stuff
        obj.debugAltitude = obj.position.magnitude;
        obj.debugRecievedGravity = obj.parent.gravParam / (obj.debugAltitude * obj.debugAltitude);
        obj.debugSurfaceGravity = 1000.0f * obj.gravParam / (obj.radius * obj.radius);
        obj.debugVelocity = obj.velocity.magnitude;
        //end debug stuff

        //the following is my attempt at a runge-kutta implementation, using two-body approximation
        Vector3d a1, a2, a3, a4;
        Vector3d v1, v2, v3, v4;
        Vector3d k1, k2, k3, k4;

        double deltaT = Time.fixedDeltaTime;

        v1 = eulerVel(obj.position, obj.velocity, deltaT, obj);
        k1 = v1 * deltaT;
        v2 = eulerVel(obj.position + (k1 / 2.0d), ((v1 - obj.velocity) / 2.0d) + obj.velocity, deltaT / 2.0d, obj);
        k2 = v2 * deltaT;
        v3 = eulerVel(obj.position + (k2 / 2.0d), ((v2 - obj.velocity) / 2.0d) + obj.velocity, deltaT / 2.0d, obj);
        k3 = v3 * deltaT;
        v4 = eulerVel(obj.position + k3, v3, deltaT, obj);
        k4 = v4 * deltaT;

        Vector3d newPos = obj.position + ((k4 + 2.0d * k3 + 2.0d * k2 + k1) / 6.0d);

        a1 = getParentGravity(obj.position, obj);
        k1 = a1 * deltaT;
        a2 = getParentGravity(obj.position + k1 * deltaT / 2, obj);
        k2 = a2 * deltaT / 2 + a1 * deltaT / 2;
        a3 = getParentGravity(obj.position + k2 * deltaT / 2, obj);
        k3 = a3 * deltaT / 2 + a2 * deltaT / 2;
        a4 = getParentGravity(obj.position + k3 * deltaT, obj);
        k4 = a4 * deltaT;

        obj.velocity = obj.velocity + (k1 + 2.0d * k2 + 2.0d * k3 + k4) / 6.0d;

        obj.position = newPos;
        obj.transform.localPosition = (Vector3)obj.position;
    }

    public override double GetSemiMajorAxis()
    {
        double specificEnergy = obj.velocity.magnitude * obj.velocity.magnitude / 2.0d - obj.parent.gravParam / obj.position.magnitude;
        return (-0.5d * obj.parent.gravParam) / specificEnergy;
    }


    //helpers
    public Vector3d getParentGravity(Vector3d mPos, CelestialObj obj)
    {
        return (-1.0d * (mPos / mPos.magnitude)) * ((obj.parent.gravParam) / (mPos.magnitude * mPos.magnitude));
    }
    public Vector3d eulerVel(Vector3d mPos, Vector3d mVel, double deltaT, CelestialObj obj)
    {
        return mVel + (getParentGravity(mPos, obj) * deltaT);
    }
}
