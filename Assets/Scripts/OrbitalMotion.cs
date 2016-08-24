using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbitalMotion : MonoBehaviour 
{

	public GameObject planetRef;
	public GameObject planetIndRef;
    public GameObject SOIIndRef;
	public Planet sun;

	public List<Planet> planets = new List<Planet>();

    // Use this for initialization
    void Start() 
	{
        addSatellites(sun, Universe.MIN_PLANET_MASS, Universe.MAX_PLANET_MASS);
	}

	void addSatellites(Planet parentPlanet, double minMass, double maxMass)
	{
        if (minMass < maxMass)
        {
            //fenceposting

            //ultimately, mass of planet should impact number of moons to some degree
            int satelliteCount = Random.Range(2, 10);

            Planet satelliteBuffer = genPlanet(minMass, maxMass);

            //initial 'used radius', based off of how much room the parent Planet itself will take up (uses roche limit)
            double usedRadius = 1.26 * satelliteBuffer.radius * Mathd.Pow(parentPlanet.mass/minMass, 1.0d/3.0d);
            Debug.Log("FUCK: " + usedRadius);

            int x = 0;

            while (((satelliteBuffer.soi * 2.0d) < (parentPlanet.soi - usedRadius)) && x < satelliteCount)
            {
                //set moons parent
                satelliteBuffer.parent = parentPlanet;
                satelliteBuffer.transform.parent = parentPlanet.transform;

                usedRadius += satelliteBuffer.soi;

                //position moon between minimum acceptable altitude and mumble mumble

                double spawnDistance = (double)Random.Range((float)(usedRadius), (float)(usedRadius + parentPlanet.soi * 0.05d));
                Vector3d spawnPosition = new Vector3d(spawnDistance, 0.0d, 0.0d);
                satelliteBuffer.position = spawnPosition;


                //compute orbital velocity (should add some tolerances for ellipticalness)
                double zvel = Mathd.Sqrt((Universe.G * parentPlanet.mass) / spawnPosition.magnitude);

                //random values should be added to other components as well ideally, that way they wont all have aligned apoapsis and periapsis
                satelliteBuffer.velocity += new Vector3d(0.0d, 0.0d, zvel);

                planets.Add(satelliteBuffer);

                usedRadius = (satelliteBuffer.soi + spawnDistance);
                
                addSatellites(satelliteBuffer, Universe.MIN_MOON_MASS, 0.3d * satelliteBuffer.mass);

                x++;

                satelliteBuffer = genPlanet(minMass, maxMass);
            }

            Destroy(satelliteBuffer.gameObject);
        }
	}

	Planet genPlanet(double minMass, double maxMass)
	{
        if (minMass < maxMass)
        {
            Planet planetBuffer = ((GameObject)Instantiate(planetRef)).GetComponent<Planet>();

            PlanetIndicator model = ((GameObject)Instantiate(planetIndRef)).GetComponent<PlanetIndicator>();

            //set mass within pre-calculated tolerances
            planetBuffer.mass = Random.Range((float)minMass, (float)maxMass);

            //define gravity parameter for somewhat cheaper gravity computations
            planetBuffer.gravParam = planetBuffer.mass * Universe.G;

            //set density
            planetBuffer.density = Random.Range((float)minDensity(planetBuffer.mass), (float)maxDensity(planetBuffer.mass));

            //give it a model
            planetBuffer.model = model;
            model.transform.parent = planetBuffer.transform; //slave it to the planet

            //order planet to decide how broad it is and set its sphere of influence based on all past data it took on
            planetBuffer.UpdateVolume();
            planetBuffer.UpdateSOI();

            //give it an SOI indicator
            GameObject indicator = ((GameObject)Instantiate(SOIIndRef));
            indicator.transform.localScale = new Vector3((float)(2.0d * planetBuffer.soi), (float)(2.0d * planetBuffer.soi), (float)(2.0d * planetBuffer.soi));
            indicator.transform.parent = planetBuffer.transform;

            //set atmosphere
            planetBuffer.UpdateAtmo(atmoRandomizer(planetBuffer.surfaceGravity, planetBuffer.radius));

            return planetBuffer;
        }
        return null;
	}

	void FixedUpdate()
	{
        /*
		foreach (Planet planet in planets)
		{
			//grab parents mass and use that to compute le gravitee
			planet.UpdatePosition(Time.fixedDeltaTime);

			planet.debugVelocity = planet.velocity.magnitude;
			planet.debugAltitude = planet.position.magnitude;
			planet.debugGravity = planet.computeAccel(planet.position).magnitude;
		}*/
	}

	double maxDensity(double mass)
	{
		return mass*0.000006963788d + 0.003493036d;
	}

	double minDensity(double mass)
	{
		return mass*0.000007518797d + 0.002992481d;
	}

	double atmoRandomizer (double gravity, double radius)
	{
		double percent = Random.value;

		double n = percent * 3619115000000.0d;

		return n * gravity * gravity / (4.0d * Mathf.PI * radius * radius);
	}
}
