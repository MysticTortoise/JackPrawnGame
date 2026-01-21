using System;
using UnityEngine;
using UnityEngine.Serialization;

public class JPFollowCamera : MonoBehaviour
{
    private Camera managedCamera;
    [FormerlySerializedAs("target")] public Transform Target;

    [FormerlySerializedAs("safeZone")] [SerializeField] private float SafeZone;
    [FormerlySerializedAs("moveSpeed")] [SerializeField] private float MoveSpeed;

    private void Start()
    {
        managedCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        JPCameraBlocker leftmostBlocker = null;
        JPCameraBlocker rightmostBlocker = null;
        foreach (JPCameraBlocker blocker in JPCameraBlocker.ActiveBlockers)
        {
            if (blocker.BlockRightMovement)
            {
                if (rightmostBlocker is null || blocker.transform.position.x < rightmostBlocker.transform.position.x)
                    rightmostBlocker = blocker;
            }
            else
            {
                if (leftmostBlocker is null || blocker.transform.position.x > leftmostBlocker.transform.position.x)
                    leftmostBlocker = blocker;
            }
        }
        
        if (Target.position.x > transform.position.x + SafeZone)
        {
            transform.position += new Vector3(MoveSpeed * Time.deltaTime, 0, 0);
        } else if (Target.position.x < transform.position.x - SafeZone)
        {
            transform.position -= new Vector3(MoveSpeed * Time.deltaTime, 0, 0);
        }

        float leftBound = leftmostBlocker is null ? float.NegativeInfinity :
            leftmostBlocker.transform.position.x + managedCamera.orthographicSize * managedCamera.aspect;
        
        float rightBound = rightmostBlocker is null ? float.PositiveInfinity :
            rightmostBlocker.transform.position.x + managedCamera.orthographicSize * managedCamera.aspect;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, leftBound, rightBound),
            transform.position.y, transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position, new Vector3(
            SafeZone * 2, 
            GetComponent<Camera>().orthographicSize * 2, 
            0));
        
        Gizmos.color = Color.red;
        managedCamera = GetComponent<Camera>();
        Gizmos.DrawWireCube(GetBounds().center, GetBounds().size);
    }

    public void SetTarget(JPCharacter character)
    {
        Target = character.transform.Find("Visuals");
    }

    public Rect GetBounds()
    {
        var rect = new Rect(
            Vector2.zero,
            new Vector2(
                managedCamera.orthographicSize * 2 * managedCamera.aspect,
                managedCamera.orthographicSize * 2));
        rect.center = new Vector2(transform.position.x, transform.position.y);
        return rect;
    }
}
