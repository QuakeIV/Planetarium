using UnityEngine;
using System.Collections;

public abstract class Universe
{
    public enum CelestialType {Terrestrial, GasGiant, Star, BlackHole};
    public const double G = 0.03978874d;
    public static double timeWarp = 1.0d;
}
