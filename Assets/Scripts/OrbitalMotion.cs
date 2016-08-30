using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbitalMotion : MonoBehaviour 
{
	public GameObject planetIndRef;
    public GameObject SOIIndRef;
	public GameObject sun;

	//public List<Planet> planets = new List<Planet>();
    
    void Start() 
	{
        buildUniverse(sun.GetComponent<CelestialObj>());
	}

	void buildUniverse(CelestialObj parentObj)
	{
        //fenceposting

        //ultimately, mass of planet should impact number of moons to some degree
        //also this whole approach is a bit iffy
        int satelliteCount = Random.Range(2, 10);

        Planet planetBuffer = Planet.NewPlanet(parentObj);

        //initial 'used radius', based off of how much room the parent Planet itself will take up (uses roche limit)
        double usedRadius = 1.26 * planetBuffer.radius * Mathd.Pow(parentObj.mass/planetBuffer.mass, 1.0d/3.0d);
        Debug.Log("Roche limit used: " + usedRadius);

        planetBuffer.position = new Vector3d(usedRadius * 1.1d, 0.0d, 0.0d);

        double zvel = Mathd.Sqrt((Universe.G * parentObj.mass) / planetBuffer.position.magnitude);
        planetBuffer.velocity = new Vector3d(0.0d, 0.0d, zvel);

        /*
        int x = 0;

        while (((satelliteBuffer.soi * 2.0d) < (parentPlanet.soi - usedRadius)) && x < satelliteCount)
        {
            usedRadius += satelliteBuffer.soi;

            //position moon between minimum acceptable altitude and mumble mumble

            double spawnDistance = (double)Random.Range((float)(usedRadius), (float)(usedRadius + parentPlanet.soi * 0.05d));
            Vector3d spawnPosition = new Vector3d(spawnDistance, 0.0d, 0.0d);
            satelliteBuffer.position = spawnPosition;


            //compute orbital velocity (should add some tolerances for ellipticalness)
            double zvel = Mathd.Sqrt((Universe.G * parentPlanet.mass) / spawnPosition.magnitude);

            //random values should be added to other components as well ideally, that way they wont all have aligned apoapsis and periapsis
            satelliteBuffer.velocity += new Vector3d(0.0d, 0.0d, zvel);

            //planets.Add(satelliteBuffer);

            usedRadius = (satelliteBuffer.soi + spawnDistance);

            x++;

            satelliteBuffer = genPlanet(minMass, maxMass);
        }

        Destroy(satelliteBuffer.gameObject);*/
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
}
