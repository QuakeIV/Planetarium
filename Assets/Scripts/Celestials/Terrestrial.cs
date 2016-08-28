using UnityEngine;
using System.Collections;

public class Terrestrial : Planet
{
    private const float MIN_MASS = 1.0f;
    private const float MAX_MASS = 400.0f;
    public double surfaceGravity;

    // Use this for initialization
    void Start()
    {
        surfaceGravity = gravParam * 1000.0f / (radius * radius);
    }

    public override void Init(CelestialObj parent)
    {
        base.Init(parent);

        transform.parent = parent.transform;
        this.parent = parent;

        type = Universe.CelestialType.Terrestrial;

        mass = Random.Range(MIN_MASS, MAX_MASS);
        density = Random.Range((float)minDensity(mass), (float)maxDensity(mass));

        gravParam = mass * Universe.G;

        UpdateRadius();
        UpdateSOI();

        //set up atmosphere
        /*
            double percent = Random.value;

		    double n = percent * 3619115000000.0d;

		    return n * gravity * gravity / (4.0d * Mathf.PI * radius * radius);
        */

        //set up visual representation
        model = gameObject.AddComponent<CelestialIndicator>();
        model.Init(this);

        //set up SOI indicator
        /*
                //give it an SOI indicator
        GameObject indicator = ((GameObject)Instantiate(SOIIndRef));
        indicator.transform.localScale = new Vector3((float)(2.0d * planetBuffer.soi), (float)(2.0d * planetBuffer.soi), (float)(2.0d * planetBuffer.soi));
        indicator.transform.parent = planetBuffer.transform;*/
    }

    public override void UpdateRadius()
    {
        radius = Mathd.Pow((mass * 3.0d) / (4.0d * Mathf.PI * density), 1.0d / 3.0d);
        //model.UpdateRadius(radius);
    }

    //very placeholdery, still unused
    public override void UpdateAtmo(double pressure)
    {
        atmoPressure = pressure;
        atmoRadius = 1.4f * radius;
        Debug.Log("UpdateAtmo Run with " + pressure);
    }

    //very placeholdery, still unused
    public override double GetAtmoPressure(double altitude)
    {
        return -atmoPressure * altitude / atmoRadius + atmoPressure;
    }

    //helpers
    private double maxDensity(double mass)
    {
        return mass * 0.000006963788d + 0.003493036d;
    }
    private double minDensity(double mass)
    {
        return mass * 0.000007518797d + 0.002992481d;
    }
}
