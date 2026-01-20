
using System;
using UnityEngine;

public class JPPlayfieldZSide : MonoBehaviour
{
    public static JPPlayfieldZSide topSide;
    public static JPPlayfieldZSide bottomSide;


    [SerializeField] private bool isTop;
    
    public JPProjectedCollider projectedCollider { get; private set; }
    
    private void OnEnable()
    {
        if (isTop)
            topSide = this;
        else
            bottomSide = this;

        projectedCollider = GetComponent<JPProjectedCollider>();
    }
}
