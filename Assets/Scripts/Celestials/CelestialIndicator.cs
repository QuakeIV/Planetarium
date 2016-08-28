using UnityEngine;
using System.Collections;

public class CelestialIndicator : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
    }

    public void Init(CelestialObj parent)
    {
        transform.parent = parent.transform;
        UpdateRadius(parent.radius);
    }

    public void UpdateRadius(double radius)
    {
        transform.localScale = new Vector3(2.0f * (float)radius, 2.0f * (float)radius, 2.0f * (float)radius);
    }
}
