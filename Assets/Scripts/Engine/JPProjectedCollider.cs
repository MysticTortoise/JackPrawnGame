using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

[Flags]
public enum JPCollisionType : uint
{
    None = 0,
    Solid = 1 << 0,
    Entity = 1 << 1,
    Hurtbox = 1 << 2,
    Hitbox = 1 << 3,
    Any = uint.MaxValue,
}

public static class JPCollisionTypeExtensions
{
    public static Color GetColor(this JPCollisionType type)
    {
        return type switch
        {
            JPCollisionType.None => Color.gray2,
            JPCollisionType.Any => Color.white,
            JPCollisionType.Solid => Color.green,
            JPCollisionType.Entity => Color.red,
            JPCollisionType.Hitbox => Color.purple,
            JPCollisionType.Hurtbox => Color.yellow,
            _ => Color.gray
        };
    }
}

public class JPProjectedCollider : MonoBehaviour
{
    [FormerlySerializedAs("offset")] public Vector3 Offset;
    [FormerlySerializedAs("extents")] public Vector3 Extents;
    [FormerlySerializedAs("collisionType")] public JPCollisionType CollisionType;
    
    public JPRect rect => new(GetCenter(), Extents);

    private static readonly List<JPProjectedCollider> ColliderList = new();

    public Vector3 GetCenter()
    {
        return transform.position + Offset;
    }

    public void OnEnable()
    {
        ColliderList.Add(this);
    }

    public void OnDisable()
    {
        ColliderList.Remove(this);
    }

    

    public List<JPProjectedCollider> CheckCollision(JPCollisionType collisionFilter)
    {
        return ColliderList
            .Where(other => (other.CollisionType & collisionFilter) != 0)
            .Where(other => rect.Intersects(other.rect))
            .ToList();
    }

    public static List<JPProjectedCollider> GetCollidersIn(JPRect bounds, JPCollisionType collisionFilter)
    {
        return ColliderList
            .Where(otherCollider => (otherCollider.CollisionType & collisionFilter) != 0)
            .Where(otherCollider => otherCollider.rect.Intersects(bounds))
            .ToList();
    }

    private float GetCollideTime(JPProjectedCollider otherCollider, Vector3 amount, out Vector3 normal)
    {
        float dxEntry, dxExit;
        float dyEntry, dyExit;
        float dzEntry, dzExit;

        if (amount.x > 0)
        {
            dxEntry = otherCollider.rect.GetMin().x - rect.GetMax().x;
            dxExit  = otherCollider.rect.GetMax().x - rect.GetMin().x;
        }
        else
        {
            dxExit  = otherCollider.rect.GetMin().x - rect.GetMax().x;
            dxEntry = otherCollider.rect.GetMax().x - rect.GetMin().x;
        }
        
        if (amount.y > 0)
        {
            dyEntry = otherCollider.rect.GetMin().y - rect.GetMax().y;
            dyExit  = otherCollider.rect.GetMax().y - rect.GetMin().y;
        }
        else
        {
            dyExit  = otherCollider.rect.GetMin().y - rect.GetMax().y;
            dyEntry = otherCollider.rect.GetMax().y - rect.GetMin().y;
        }
        
        if (amount.z > 0)
        {
            dzEntry = otherCollider.rect.GetMin().z - rect.GetMax().z;
            dzExit  = otherCollider.rect.GetMax().z - rect.GetMin().z;
        }
        else
        {
            dzExit  = otherCollider.rect.GetMin().z - rect.GetMax().z;
            dzEntry = otherCollider.rect.GetMax().z - rect.GetMin().z;
        }
        
        /*JPDebugDrawer.AddCommand(new JPRayDrawCommand(
            new Vector3(rect.GetMax().x, 0, 0), 
            new Vector3(otherCollider.rect.GetMin().x,0,0),
            Color.red));*/

        float txEntry, txExit;
        float tyEntry, tyExit;
        float tzEntry, tzExit;

        if (amount.x == 0)
        {
            txEntry = float.NegativeInfinity;
            txExit = float.PositiveInfinity;
        }
        else
        {
            txEntry = dxEntry / amount.x;
            txExit = dxExit / amount.x;
        }
        
        if (amount.y == 0)
        {
            tyEntry = float.NegativeInfinity;
            tyExit = float.PositiveInfinity;
        }
        else
        {
            tyEntry = dyEntry / amount.y;
            tyExit = dyExit / amount.y;
        }
        
        if (amount.z == 0)
        {
            tzEntry = float.NegativeInfinity;
            tzExit = float.PositiveInfinity;
        }
        else
        {
            tzEntry = dzEntry / amount.z;
            tzExit = dzExit / amount.z;
        }
        
        float entryTime = Mathf.Max(Mathf.Max(txEntry, tyEntry), tzEntry);
        float exitTime = Mathf.Max(Mathf.Max(txExit, tyExit), tzExit);

        if (entryTime > exitTime || 
            txEntry < 0 && tyEntry < 0 && tzEntry < 0 || 
            txEntry > 1 || tyEntry > 1 || tzEntry > 1)
        {
            // No Collision
            normal = Vector3.zero;
            return 1.0f;
        }
        // There is one!
        if (Mathf.Approximately(entryTime, txEntry))
            normal = new Vector3(-Mathf.Sign(txEntry), 0, 0);
        else if (Mathf.Approximately(entryTime, tyEntry))
            normal = new Vector3(0, -Mathf.Sign(tyEntry), 0);
        else
            normal = new Vector3(0, 0, -Mathf.Sign(tzEntry));
        return entryTime;
    }

    public void MoveCollider(Vector3 amount, uint depth = 10)
    {
        if (depth == 0)
            return;


        var collidersToCheck = GetCollidersIn(
            JPRect.FromMinMax(
                Vector3.Min(rect.GetMin(), rect.GetMin() + amount),
                Vector3.Max(rect.GetMax(), rect.GetMax() + amount)),
            JPCollisionType.Solid).Where(otherCollider => otherCollider != this);

        float collideTime = 1;
        Vector3 finalNormal = Vector3.zero;

        foreach (JPProjectedCollider otherCollider in collidersToCheck)
        {
            float newTime = GetCollideTime(otherCollider, amount, out Vector3 normal);
            if (!(newTime < collideTime)) continue;
            collideTime = newTime;
            finalNormal = normal;
        }

        transform.position += (amount * collideTime);
        Vector3 newTryVel = amount * (1-collideTime);
        newTryVel = Vector3.Scale(newTryVel, new Vector3(
            1.0f - Math.Abs(finalNormal.x),
            1.0f - Math.Abs(finalNormal.y),
            1.0f - Math.Abs(finalNormal.z)
        ));
        MoveCollider(newTryVel, depth-1);

    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = CollisionType.GetColor();
        Vector2 projectedScale = JPProjection.projectPoint(Extents * 2);
        
        
        Gizmos.DrawWireCube(JPProjection.projectPoint(GetCenter(), FindObjectsByType<JPParallaxFloor>(FindObjectsSortMode.None).First(p => p.primary)), projectedScale);
    }
#if UNITY_EDITOR

    protected void OnDrawGizmos()
    {
        Gizmos.color = CollisionType.GetColor();
        
        if (Camera.current != null && (!Camera.current.orthographic || Camera.current.transform.rotation * Vector3.forward == Vector3.down))
        {
            Gizmos.DrawCube(GetCenter(), Extents * 2);
        }
        
        //Gizmos.DrawWireSphere(JPProjection.projectPoint(GetCenter(), FindAnyObjectByType<JPParallaxFloor>()), 1f);
    }
    #endif

#endif
}
