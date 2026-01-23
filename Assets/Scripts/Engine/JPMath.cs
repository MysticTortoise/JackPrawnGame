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
    
    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
