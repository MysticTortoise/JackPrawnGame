using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class JPSpriteProjected : MonoBehaviour
{
    [SerializeField] private JPProjectedCollider baseCollider;
    private JPParallaxFloor mainFloor;

    private Dictionary<SpriteRenderer, int> children = new();

    private void LookForCamera()
    {
        mainFloor = FindObjectsByType<JPParallaxFloor>(FindObjectsSortMode.None).First();
    }

    private void Start()
    {
        LookForCamera();
        foreach (SpriteRenderer child in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            children.Add(child, child.sortingOrder);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!baseCollider)
            return;
        
        if(!mainFloor)
            LookForCamera();
        
        
        transform.position = JPProjection.projectPoint(baseCollider.GetCenter(), mainFloor);

        if (!Application.isPlaying)
            return;
        
        foreach (var spriteRenderer in children)
        {
            spriteRenderer.Key.sortingOrder = 
                (((int)(baseCollider.GetCenter().z * 100) + 500) * -1) + spriteRenderer.Value;
        }
    }
}
