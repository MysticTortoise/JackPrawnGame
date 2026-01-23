
using System;
using System.Linq;
using JPDebugDraw;
using Unity.VisualScripting;
using UnityEngine;

public class JPForceInsideCamera : MonoBehaviour
{
    private JPFollowCamera followCam;
    private Camera stayInCamera;
    private JPParallaxFloor floor;

    private bool waitTillInCamera = false;
    private bool shouldTrack = true;

    public void Detach()
    {
        shouldTrack = false;
    }

    public void ReAttachWhenInBounds()
    {
        shouldTrack = true;
        waitTillInCamera = true;
    }

    public void ReAttach()
    {
        shouldTrack = true;
        waitTillInCamera = false;
    }
    

    private void Start()
    {
        followCam = FindAnyObjectByType<JPFollowCamera>();
        stayInCamera = followCam.GetComponentInChildren<Camera>();
        floor = FindObjectsByType<JPParallaxFloor>(FindObjectsSortMode.None).First(p => p.primary);
    }

    private void Update()
    {
        var camBounds = new Bounds(
            stayInCamera.transform.position,
            new Vector3(
                stayInCamera.orthographicSize * stayInCamera.aspect * 2, 
                0, 0
                ));

        float minPoint = JPProjection.deprojectPointAtY0(
            new Vector2(camBounds.min.x, JPProjection.projectPoint(transform.position).y), floor).x;
        float maxPoint = JPProjection.deprojectPointAtY0(
            new Vector2(camBounds.max.x, JPProjection.projectPoint(transform.position).y), floor).x;

        bool inBounds = transform.position.x >= minPoint && transform.position.x <= maxPoint;
        

        if (shouldTrack && waitTillInCamera && inBounds)
            waitTillInCamera = false;

        if (shouldTrack && !waitTillInCamera && !inBounds)
        {
            if (transform.position.x < minPoint)
                transform.position = new Vector3(minPoint, transform.position.y, transform.position.z);
            else if (transform.position.x > maxPoint)
                transform.position = new Vector3(maxPoint, transform.position.y, transform.position.z);
        }
        
    }
}
