using System;
using UnityEngine;

[Serializable]
public class JPRect
{
    public Vector3 Position, Extents;

    public JPRect(Vector3 position, Vector3 extents)
    {
        this.Position = position;
        this.Extents = extents;
    }

    public static JPRect FromMinMax(Vector3 min, Vector3 max)
    {
        return new JPRect(
            new Vector3(
                (min.x + max.x) / 2,
                (min.y + max.y) / 2,
                (min.z + max.z) / 2),
            new Vector3(
                (max.x - min.x) / 2,
                (max.y - min.y) / 2,
                (max.z - min.z) / 2));
    }
    
    public Vector3 GetMax()
    {
        return Position + Extents;
    }

    public Vector3 GetMin()
    {
        return Position - Extents;
    }

    public bool Intersects(JPRect other)
    {
        Vector3 minMe = GetMin();
        Vector3 maxMe = GetMax();

        Vector3 minOther = other.GetMin();
        Vector3 maxOther = other.GetMax();

        return (
            minMe.x <= maxOther.x &&
            maxMe.x >= minOther.x &&
            minMe.y <= maxOther.y &&
            maxMe.y >= minOther.y &&
            minMe.z <= maxOther.z &&
            maxMe.z >= minOther.z
        );
    }
    
    public bool Intersects(Vector3 point)
    {
        var min = GetMin();
        var max = GetMax();
        
        return (
            point.x <= max.x &&
            point.x >= min.x &&
            point.y <= max.y &&
            point.y >= min.y &&
            point.z <= max.z &&
            point.z >= min.z
        );
    }
}