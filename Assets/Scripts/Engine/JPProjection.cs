using JPDebugDraw;
using UnityEngine;

public class JPProjection
{
    public const float zScale = .5f;

    public static Vector2 projectPoint(Vector3 position)
    {
        return new Vector2(position.x, position.y + (position.z * zScale));
    }

    public static Vector2 projectPoint(Vector3 position, JPParallaxFloor floor)
    {
        Vector2 pos = projectPoint(position);
        pos.x += (position.x - floor.transform.position.x) * JPMath.InverseLerpUnclamped(floor.zTop, floor.zBottom, position.z) * floor.zSlope;
        return pos;
    }

    public static Vector2 deprojectPointAtY0(Vector2 position)
    {
        return new Vector2(position.x, position.y / zScale);
    }

    public static Vector2 deprojectPointAtY0(Vector2 position, JPParallaxFloor floor)
    {
        Vector2 pos = deprojectPointAtY0(position);
        float t = (pos.y - floor.zTop) / (floor.zBottom - floor.zTop) * floor.zSlope;
        pos.x = ((position.x / t) + floor.transform.position.x) / ((1 / t) + 1);
        return pos;
    }

    public static Vector3 getScaleAxis()
    {
        return new Vector3(1, 1, zScale);
    }


}
