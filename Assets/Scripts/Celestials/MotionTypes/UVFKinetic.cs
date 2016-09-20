using UnityEngine;
using System;

/*
Universal Variable Formulation solution to finding orbital posotions

this motion type functions by 
*/
public class UVFKinetic : OrbitType
{
    private Vector3d startPosition;
    private Vector3d startVelocity;
    private double startRadialVelocity;
    private double currentTime;
    private double specificEnergy;
    private double semiMajorAxis;
    private double period;
    private double alpha;

    public UVFKinetic(CelestialObj pObj, Vector3d pPos, Vector3d pVel)
    {
        if ((pPos == null) || (pVel == null))
        {
            throw new NullReferenceException("Must get mover instance after having set up position and velocity!");
        }

        if(pObj.parent == null)
        {
            throw new NullReferenceException("UVFKinetic type mover requires that a parent object be defined");
        }

        obj = pObj;
        startPosition = new Vector3d(pPos);
        startVelocity = new Vector3d(pVel);
        startRadialVelocity = Vector3d.Dot(pPos, pVel) / pPos.magnitude;

        recomputeParameters();
    }

    public override void UpdatePosition()
    {
        double param = brentMethod();

        obj.position = computePosition(param);
        obj.velocity = computeVelocity(param);

        //try to avoid precision loss due to large time values
        if (currentTime >= period)
        {
            currentTime = currentTime % period;
        }

        obj.transform.localPosition = (Vector3)obj.position;

        currentTime += Time.fixedDeltaTime * 1000.0d;

        obj.debugAltitude = obj.position.magnitude;
        obj.debugRecievedGravity = obj.parent.gravParam / (obj.debugAltitude * obj.debugAltitude);
        obj.debugSurfaceGravity = obj.gravParam / (obj.radius * obj.radius);
        obj.debugVelocity = 0.0d;
    }

    //orbital parameter return functions

    public override double GetSemiMajorAxis()
    {
        specificEnergy = startVelocity.magnitude * startVelocity.magnitude / 2.0d - obj.parent.gravParam / startPosition.magnitude;
        semiMajorAxis = (-0.5d * obj.parent.gravParam) / specificEnergy;
        return semiMajorAxis;
    }


    //various helpers
    private Vector3d computePosition(double s)
    {
        return startPosition * positionHelperOne(s) + startVelocity * positionHelperTwo(s);
    }
    private double positionHelperOne(double s)
    {
        return 1.0d - (obj.parent.gravParam / startPosition.magnitude) * s * s * stumpff2(alpha * s * s);
    }
    private double positionHelperTwo(double s)
    {
        return currentTime - obj.parent.gravParam * s * s * s * stumpff3(alpha * s * s);
    }

    private Vector3d computeVelocity(double s)
    {
        return new Vector3d(Vector3.zero);
    }
    private double velocityHelperOne(double s)
    {
#warning not implemented
        return 0;
    }
    private double velocityHelperTwo(double s)
    {
#warning not implemented
        return 0;
    }
    
    private double brentMethod()
    {
        double guessVal = (Mathd.Sqrt(obj.parent.gravParam) * currentTime) / semiMajorAxis;

        //second two params are the min and max values the solver will search for, they are at this point total spitballing
        Accord.Math.Optimization.BrentSearch searcher = new Accord.Math.Optimization.BrentSearch(substitutionFunction, -2.0d * (guessVal + 20.0d), 2.0d*(guessVal + 20.0d));

        searcher.Tolerance = 0.0001;
        searcher.FindRoot();
        return searcher.Solution;
    }
    private double substitutionFunction(double val)
    {
        return 
            startPosition.magnitude * val * stumpff1(alpha * val * val) +
            startPosition.magnitude * startRadialVelocity * val * val * stumpff2(alpha * val * val) +
            obj.parent.gravParam * val * val * val * stumpff3(alpha * val * val) - 
            currentTime;
    }
    private double stumpff0(double val)
    {
        if (val == 0.0d)
            return 0.0d;

        if(val > 0.0d)
        {
            return Mathd.Cos(Mathd.Sqrt(val));
        }
        else
        {
            val = Mathd.Sqrt(-val);
            return (Mathd.Exp(val) + Mathd.Exp(-val)) / 2.0d;
        }
    }
    private double stumpff1(double val)
    {
        if (val == 0.0d)
            return 0.0d;

        if (val > 0.0d)
        {
            return Mathd.Sin(Mathd.Sqrt(val)) / Mathd.Sqrt(val);
        }
        else
        {
            val = Mathd.Sqrt(-val);
            return ((Mathd.Exp(val) - Mathd.Exp(-val)) / 2.0d) / val;
        }
    }
    private double stumpff2(double val)
    {
        if (val == 0.0d)
            return 0.0d;

        return (1.0d -stumpff0(val)) / val;
    }
    private double stumpff3(double val)
    {
        if (val == 0.0d)
            return 0.0d;

        return (1.0d -stumpff1(val)) / val;
    }

    private void recomputeParameters()
    {
        specificEnergy = obj.velocity.magnitude * obj.velocity.magnitude / 2.0d - obj.parent.gravParam / startPosition.magnitude;
        semiMajorAxis = (-0.5d * obj.parent.gravParam) / specificEnergy;
        period = 2.0d * Mathd.PI * semiMajorAxis * Mathd.Sqrt(semiMajorAxis / obj.parent.gravParam);
        alpha = obj.parent.gravParam / semiMajorAxis;
    }
}
