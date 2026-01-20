using UnityEditor;
using UnityEngine;
public  static class JPHandlesDrawExtensions
{
    static Vector3 cachedCenter;
    static Vector3 cachedSize;
    static Vector3[] cachedVerts = new Vector3[8];
    public static void DrawSolidCube(Vector3 center, Vector3 size)
    {

        Vector3 half = size * 0.5f;

        if (center != cachedCenter || size != cachedSize)
        {
            cachedVerts[0] = center + new Vector3(-half.x, -half.y, -half.z);
            cachedVerts[1] = center + new Vector3(half.x, -half.y, -half.z);
            cachedVerts[2] = center + new Vector3(half.x, half.y, -half.z);
            cachedVerts[3] = center + new Vector3(-half.x, half.y, -half.z);
            cachedVerts[4] = center + new Vector3(-half.x, -half.y, half.z);
            cachedVerts[5] = center + new Vector3(half.x, -half.y, half.z);
            cachedVerts[6] = center + new Vector3(half.x, half.y, half.z);
            cachedVerts[7] = center + new Vector3(-half.x, half.y, half.z);

            cachedCenter = center;
            cachedSize = size;
        }

        // Define each face with 4 vertices
        DrawQuad(cachedVerts[0], cachedVerts[1], cachedVerts[2], cachedVerts[3]); // Back
        DrawQuad(cachedVerts[5], cachedVerts[4], cachedVerts[7], cachedVerts[6]); // Front
        DrawQuad(cachedVerts[4], cachedVerts[0], cachedVerts[3], cachedVerts[7]); // Left
        DrawQuad(cachedVerts[1], cachedVerts[5], cachedVerts[6], cachedVerts[2]); // Right
        DrawQuad(cachedVerts[3], cachedVerts[2], cachedVerts[6], cachedVerts[7]); // Top
        DrawQuad(cachedVerts[4], cachedVerts[5], cachedVerts[1], cachedVerts[0]); // Bottom
    }

    public static void DrawQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Handles.DrawAAConvexPolygon(a, b, c, d);
    }
}