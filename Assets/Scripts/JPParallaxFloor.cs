using System;
using UnityEngine;

public class JPParallaxFloor : MonoBehaviour
{
    private static readonly int Scale = Shader.PropertyToID("_Scale");
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float scale; 
    public bool primary = true;
    
    public float zTop;
    public float zBottom;
    public float zSlope;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.material.SetFloat("_EffectStrength", distortStrength);
    }

    private void Update()
    {
        spriteRenderer.material.SetFloat(Scale, JPMath.nfmod(transform.position.x * scale / spriteRenderer.bounds.size.x, 1));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawRay(
            JPProjection.projectPoint(
                new Vector3(-100,0,zTop)),
            new Vector3(200, 0, 0));
        
        Gizmos.DrawRay(
            JPProjection.projectPoint(
                new Vector3(-100,0,zBottom)),
            new Vector3(200, 0, 0));
    }
}
