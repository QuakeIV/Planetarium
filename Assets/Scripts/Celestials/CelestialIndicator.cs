using UnityEngine;
using System.Collections;

public class CelestialIndicator : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
    }

    public void UpdateRadius(double radius)
    {
        transform.localScale = new Vector3(2.0f * (float)radius, 2.0f * (float)radius, 2.0f * (float)radius);
    }
}
