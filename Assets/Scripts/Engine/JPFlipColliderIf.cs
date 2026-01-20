
using System;
using UnityEngine;

public class JPFlipColliderIf : MonoBehaviour
{
    [SerializeField] private Transform FlipIf;
    private float xStart;
    private JPProjectedCollider projectedCollider;

    private void Start()
    {
        projectedCollider = GetComponent<JPProjectedCollider>();
        xStart = projectedCollider.Offset.x;
    }

    private void Update()
    {
        projectedCollider.Offset = new Vector3(
            xStart * Mathf.Sign(FlipIf.localScale.x),
            projectedCollider.Offset.y,
            projectedCollider.Offset.z
        );
    }
}
