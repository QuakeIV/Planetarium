using UnityEngine;
using System.Collections;

public class PlanetIndicator : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
	}

    public void Init(CelestialObj parent)
    {

    }

	public void UpdateRadius (double radius)
	{
		transform.localScale = new Vector3(2.0f * (float)radius, 2.0f * (float)radius, 2.0f * (float)radius);
	}
}
