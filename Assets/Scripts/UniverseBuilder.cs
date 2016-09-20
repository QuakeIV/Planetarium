using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

public abstract class UniverseBuilder
{
    public static void BuildUniverse()
    {
        Blueprint starBP = new StarBlueprint(200000.0d, 0.00000014d, 0.0d, 0.0d, 0.0d, null);

        SpecSystem(starBP);

        InstantiateSystem(starBP, null);

        //for now build one sun of hardcoded mass and density


        /*TerrestrialPlanet planet = UniverseBuilder.BuildCelestial<TerrestrialPlanet>(star, 1.3 * star.radius, 0.0d);

        //initial 'used radius', based off of how much room the parent Planet itself will take up (uses roche limit)
        //double usedRadius = 1.26 * planet.radius * Mathd.Pow(star.mass / planet.mass, 1.0d / 3.0d);
        //Debug.Log("Roche limit used: " + usedRadius);
        
        double usedRadius = planet.radius * 5.0d;

        TerrestrialPlanet moon1 = UniverseBuilder.BuildCelestial<TerrestrialPlanet>(planet, usedRadius, planet.mass * 0.4d);
        usedRadius = moon1.soi * 2.0d + 15.0d;

        TerrestrialPlanet moon2 = UniverseBuilder.BuildCelestial<TerrestrialPlanet>(planet, usedRadius, planet.mass * 0.4d);
        usedRadius = moon1.radius * 5.0d;

        TerrestrialPlanet submoon = UniverseBuilder.BuildCelestial<TerrestrialPlanet>(moon2, usedRadius, moon2.mass * 0.4d);*/
    }

    //meant to generate a 'system' of planets around a given star
    //not really based on anything in particular right now, just played around with the geometry to get something that was passable by visual inspection
    private static void SpecSystem(Blueprint star)
    {
        #warning find a double precision option
        int planetcount = UnityEngine.Random.Range(2, 8);

        double starRadius = star.getRadius();
        double usedRadius = UnityEngine.Random.Range((float)(starRadius * 2.0d), (float)(starRadius * 5.0d));

        for (int i = 0; i < planetcount; i++)
        {
            double mass = UnityEngine.Random.Range((float)TerrestrialBlueprint.getMinMass(), (float)TerrestrialBlueprint.getMaxMass());
            double density = UnityEngine.Random.Range((float)TerrestrialBlueprint.getMinDensity(mass), (float)TerrestrialBlueprint.getMaxDensity(mass));
            double startAltitude = usedRadius;
            double startSpeed = Mathd.Sqrt((Universe.G * star.mass) / startAltitude);
            startSpeed = UnityEngine.Random.Range((float)(startSpeed), (float)(startSpeed * 1.05d));
            double startAngle = UnityEngine.Random.Range(0.0f, (float)(Mathd.PI * 2.0d));
            Blueprint planet = new TerrestrialBlueprint(mass, density, startAltitude, startSpeed, startAngle, star);
            star.children.AddLast(planet);

#warning this should account for the apoapsis, which is not currently equivalent to the start altitude used to set the usedRadius
            usedRadius += planet.soi * 2.0d * 1.1d; //pick position of the next planet based off of the previous planet
            usedRadius += UnityEngine.Random.Range(0.0f, (float)(planet.soi * 0.1d));
        }
    }

    //call with the root celestial object and null in order to recursively instantiate a solar system
    private static void InstantiateSystem(Blueprint rootCelestial, CelestialObj parent)
    {
        CelestialObj newParent = BuildCelestial(parent, rootCelestial);

        foreach(Blueprint child in rootCelestial.children)
        {
            InstantiateSystem(child, newParent);
        }
    }


    /*may need this syntax later, hiding it here because im a lazy bum
    private static T BuildCelestial<T>(CelestialObj parent, Blueprint bp)
    where T : CelestialObj
    {
    */


    //this is all a horrible mess of a procedure, move it into a generic under the hood method
    //if passed a null CelestialObj, this indicates that there is no parent (it also implicitly understands that it is creating the root object of the game world)
    private static CelestialObj BuildCelestial(CelestialObj parent, Blueprint bp)
    {
        GameObject gameObj = new GameObject();

        if(!(bp.type.IsSubclassOf(typeof(CelestialObj))))
        {
            throw new Exception("Celestial Blueprint class has improperly set up type!");
        }

        CelestialObj celestial = (CelestialObj)gameObj.AddComponent(bp.type);
        
        gameObj.name = bp.typeName;


        //slave the planet to its parent transform, tell the script who that is
        celestial.parent = parent;
        //if no parent, dont try to access components of parent
        if(parent != null)
        {
            gameObj.transform.parent = parent.transform;
        }

        //generate planets parameters
        //decide on which max mass to use
        celestial.mass = bp.mass;
        celestial.density = bp.density;

        double xposition = bp.startAltitude * Mathd.Cos(bp.startAngle);
        double zposition = bp.startAltitude * Mathd.Sin(bp.startAngle);
        celestial.position = new Vector3d(xposition, 0.0d, zposition);

#warning should add a way to perturb the orbit, as in change its plane away from the ecliptic
        //put together the velocity vector
        double xvelocity = bp.startSpeed * Mathd.Cos(bp.startAngle + Mathd.PI/4.0d);
        double zvelocity = bp.startSpeed * Mathd.Sin(bp.startAngle + Mathd.PI/4.0d);
        celestial.velocity = new Vector3d(xvelocity, 0.0d, zvelocity);

        //set up the planets ability to move around
        celestial.SetupOrbit();

        //set up atmosphere
        /*
            double percent = Random.value;

		    double n = percent * 3619115000000.0d;

		    return n * gravity * gravity / (4.0d * Mathf.PI * radius * radius);
        */



        //set up visual representation
        GameObject modelObj = new GameObject();
        modelObj.transform.parent = gameObj.transform;
        modelObj.name = "Model";

        //create a mesh and add it to the model object
        GameObject meshObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Mesh mesh = meshObj.GetComponent<MeshFilter>().sharedMesh;
        MeshFilter meshFilter = modelObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        GameObject.Destroy(meshObj);

        //add mesh renderer
        GameObject materialObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Material diffuse = materialObj.GetComponent<MeshRenderer>().sharedMaterial;
        GameObject.Destroy(materialObj);
        MeshRenderer renderer = modelObj.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = diffuse;

        celestial.model = modelObj.AddComponent<CelestialIndicator>();

        addTrail(modelObj, bp.trailColor);

        //set up SOI indicator (should be part of the model object)
        /*
                //give it an SOI indicator
        GameObject indicator = ((GameObject)Instantiate(SOIIndRef));
        indicator.transform.localScale = new Vector3((float)(2.0d * planetBuffer.soi), (float)(2.0d * planetBuffer.soi), (float)(2.0d * planetBuffer.soi));
        indicator.transform.parent = planetBuffer.transform;*/

        celestial.UpdateParameters();

        return celestial;
    }

    //helpers

    //invoke this to add a pretty trail to visualize movement
    private static void addTrail(GameObject model)
    {
        addTrail(model, new Color(255.0f, 0.0f, 0.0f, 1.0f));
    }
    private static void addTrail(GameObject model, Color clr)
    {
        if (model.GetComponent<TrailRenderer>() == null)
        {
            TrailRenderer trail = model.AddComponent<TrailRenderer>();
            trail.time = 360;
            trail.startWidth = 50;
            trail.endWidth = 25;
            Material trailcolor = trail.material;
            trailcolor.shader = Shader.Find("Unlit/Color");
            trailcolor.SetColor("_Color", clr);
            trail.material = trailcolor;
        }
    }


    //the paramaterless constructor is NOT meant to be an option for this class
    /*
    this class is meant to act as a sort of filter between system generation 
    code (which will probably get written and rewritten constantly) and the
    game environment, which will require properly instantiated objects
    */
    private abstract class Blueprint
    {
        public LinkedList<Blueprint> children;
        public Type type;
        public Color trailColor;
        public string typeName;
        public double startAngle; //in radians
        public double startAltitude;
        public double startSpeed;
        public double mass;
        public double density;
        public double soi;

        public Blueprint(double pMass, double pDensity, double pStartAltitude, double pStartSpeed, double pStartAngle, Blueprint parent)
        {
            children = new LinkedList<Blueprint>();

            mass = pMass;
            density = pDensity;
            startAltitude = pStartAltitude;
            startSpeed = pStartSpeed;
            startAngle = pStartAngle;

            if (parent != null)
            {
                //conveluted math to compute the sphere of influence
                //may be possible to find a less labor intensive version but its a constructor who cares
                double specificEnergy = (startSpeed * startSpeed) / 2.0d - (parent.mass * Universe.G) / startAltitude;
                double semiMajorAxis = (-0.5d * (parent.mass * Universe.G)) / specificEnergy;
                soi = semiMajorAxis * Mathd.Pow(parent.mass / mass, 2.0d / 5.0d);
            }
            else
            {
                soi = double.PositiveInfinity;
            }
        }

        public double getRadius()
        {
            return Mathd.Pow((mass * 3.0d) / (4.0d * Mathd.PI * density), 1.0d / 3.0d);
        }

#warning Blueprint: C# does not allow you to mandate the presence of static methods, read the following for this class
        /*all examples of this shall have the following implemented as static methods
        public static abstract double getMaxMass();
        public static abstract double getMinMass();
        public static abstract double getMaxDensity(double pMass);
        public static abstract double getMinDensity(double pMass);*/
    }

    private class TerrestrialBlueprint : Blueprint
    {
        public TerrestrialBlueprint(double pMass, double pDensity, double pStartAltitude, double pStartSpeed, double pStartAngle, Blueprint parent) : base(pMass, pDensity, pStartAltitude, pStartSpeed, pStartAngle, parent)
        {
            type = typeof(TerrestrialPlanet);
            typeName = "Terrestrial";
        }

        public static double getMaxMass()
        {
            return 400.0d;
        }

        public static double getMinMass()
        {
            return 1.0d;
        }

        public static double getMaxDensity(double pMass)
        {
            return pMass * 0.000006963788d + 0.003493036d;
        }

        public static double getMinDensity(double pMass)
        {
            return pMass * 0.000007518797d + 0.002992481d;
        }
    }

    private class StarBlueprint : Blueprint
    {
        public StarBlueprint(double pMass, double pDensity, double pStartAltitude, double pStartSpeed, double pStartAngle, Blueprint parent) : base(pMass, pDensity, pStartAltitude, pStartSpeed, pStartAngle, parent)
        {
            type = typeof(Star);
            typeName = "Star";
        }

        //density/mass specs are cheesy bullshit for now, should do research on stars later to make this more... reasonable
        //currently stars are far too dense and too small and the mass range is too small and really its just terrible
        public static double getMaxMass()
        {
            return 300000.0d;
        }

        public static double getMinMass()
        {
            return 100000.0d;
        }

        public static double getMaxDensity(double pMass)
        {
            return 0.0000001d;
        }

        public static double getMinDensity(double pMass)
        {
            return 0.0000002d;
        }
    }
}
