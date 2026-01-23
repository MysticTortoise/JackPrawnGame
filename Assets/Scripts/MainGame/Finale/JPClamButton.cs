
using System;
using UnityEngine;

public class JPClamButton : JPHurtableBox
{
    private static readonly int Pressed = Animator.StringToHash("Pressed");
    private Animator animator;
    
    [NonSerialized] public bool hit;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool(Pressed, hit);
    }

    public override bool Hit(JPCharacter source, JPCharacterAttack attack)
    {
        if (source.Faction != JPCharacterFaction.Player)
            return false;

        if (hit) return false;
        
        hit = true;

        return true;
    }
}
