using UnityEngine;
using System.Collections;

public class PlanetIndicator : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}	

	public void UpdateRadius (double radius)
	{
		transform.localScale = new Vector3(2.0f * (float)radius, 2.0f * (float)radius, 2.0f * (float)radius);
	}
}
