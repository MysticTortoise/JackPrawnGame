
using System;
using UnityEngine;

public class JPTutorialUIPopup : MonoBehaviour
{
    private static readonly int Present = Animator.StringToHash("Present");

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }

    private void Update()
    {
        animator.SetBool(Present, JPTutorialPopup.activePopup);
        if(JPTutorialPopup.activePopup)
            spriteRenderer.sprite = JPTutorialPopup.activePopup.Sprite;
    }
}
