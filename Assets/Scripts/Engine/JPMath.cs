using UnityEngine;


public class JPMath
{
    public static float nfmod(float a,float b)
    {
        return a - b * Mathf.Floor(a / b);
    }

    public static float InverseLerpUnclamped(float a, float b, float t)
    {
        return (t - a) / (b - a);
    }
}
