using System;
using UnityEngine;
using UnityEngine.Serialization;

public class JPScrollingBG : MonoBehaviour
{
    private static readonly int Amount = Shader.PropertyToID("_Amount");
    private static readonly int MainTex = Shader.PropertyToID("_Tex1");
    private static readonly int MainTex2 = Shader.PropertyToID("_Tex2");
    
    private SpriteRenderer spriteRenderer;

    [FormerlySerializedAs("speed")] [SerializeField] private float Speed;

    [SerializeField] private Texture[] Textures;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {

        float amnt = transform.position.x * Speed / spriteRenderer.bounds.size.x * 0.10428f;
        spriteRenderer.material.SetTexture(MainTex, Textures[
            ( (int)((amnt+1)/2) * 2) % Textures.Length]);
        spriteRenderer.material.SetTexture(MainTex2, Textures[
            ( (int)(amnt/2) * 2 + 1) % Textures.Length]);

        /*
        Debug.Log("First");
        Debug.Log(( (int)((amnt+1)/2) * 2) % Textures.Length);
        Debug.Log("Second");
        Debug.Log(( (int)(amnt/2) * 2 + 1) % Textures.Length);*/
        
        spriteRenderer.material.SetFloat(Amount,amnt);
    }
}
