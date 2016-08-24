using UnityEngine;
using System.Collections;

abstract class Universe : MonoBehaviour
{
    public const double G = 0.03978874d;
    public const double MIN_PLANET_MASS = 1.0d;
    public const double MIN_MOON_MASS = 0.2d;
    public const double MAX_PLANET_MASS = 400.0d;
}
