
using System;
using JPDebugDraw;
using UnityEngine;

public class JPForceInsideCamera : MonoBehaviour
{
    private Camera stayInCamera;
    private JPParallaxFloor floor;

    private void Start()
    {
        stayInCamera ??= FindAnyObjectByType<JPFollowCamera>().GetComponentInChildren<Camera>();
        floor ??= FindAnyObjectByType<JPParallaxFloor>();
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
        
        
        if (transform.position.x < minPoint)
            transform.position = new Vector3(minPoint, transform.position.y, transform.position.z);
        else if (transform.position.x > maxPoint)
            transform.position = new Vector3(maxPoint, transform.position.y, transform.position.z);
    }
}
