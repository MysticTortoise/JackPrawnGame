using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class JPPlayer : JPCharacter
{
    private static readonly int DoJab = Animator.StringToHash("DoJab");
    private static readonly int JabNum = Animator.StringToHash("JabNum");
    private static readonly int DoUpper = Animator.StringToHash("DoUpper");


    private int jabCount;
    protected JPPlayerAttackState attackState;

    [SerializeField] private float UppercutForce;
    [SerializeField] private float UppercutForwardDir;
    [SerializeField] private JPCharacterAttack[] JabAttacks;
    [SerializeField] private JPCharacterAttack UppercutAttack;
    
    private JPProjectedCollider jabHitbox;

    public override bool CanMove()
    {
        return attackState.CanMove() && base.CanMove();
    }

    public override bool CanAttack()
    {
        return attackState.CanAttack() && base.CanAttack();
    }

    protected new void Start()
    {
        base.Start();
        Faction = JPCharacterFaction.Player;

        jabHitbox = transform.Find("JabHitbox").GetComponent<JPProjectedCollider>();
    }

    protected override Vector3 SolveVelocity()
    {
        Vector3 velocity = base.SolveVelocity();
        if (attackState == JPPlayerAttackState.Uppercut)
        {
            velocity.x = (facingDir ? 1 : -1) * UppercutForwardDir;
        }
        return velocity;
    }

    protected new void Update()
    {
        base.Update();

        if (attackState == JPPlayerAttackState.Uppercut)
        { // Uppercut processing
            if(grounded)
                UppercutLanded();
            else if (yVel > 0)
            {
                foreach (JPProjectedCollider hurtbox in jabHitbox.CheckCollision(JPCollisionType.Hurtbox))
                {
                    if(hurtbox is JPHurtableBox hurtableBox)
                        hurtableBox.Hit(this, UppercutAttack);
                }
            }
        }
    }
    
    

    private bool queuedJab;

    private void Jab()
    {
        queuedJab = false;
        attackState = jabCount switch
        {
            0 => JPPlayerAttackState.Jab1,
            1 => JPPlayerAttackState.Jab2,
            2 => JPPlayerAttackState.Jab3,
            _ => attackState
        };
        jabCount++;
        animator.SetTrigger(DoJab);
        animator.SetInteger(JabNum, jabCount);
    }

    public void DoJabHitbox()
    {
        foreach (JPProjectedCollider hurtbox in jabHitbox.CheckCollision(JPCollisionType.Hurtbox))
        {
            if(hurtbox is JPHurtableBox hurtableBox)
                hurtableBox.Hit(this, JabAttacks[jabCount-1]);
        }
    }

    public void CheckQueuedJab(bool freeMovement = true)
    {
        if (queuedJab && jabCount < 3 && CanAttack())
            Jab();
        else if(freeMovement || !CanAttack())
        {
            attackState = JPPlayerAttackState.Idle;
            jabCount = 0;
        }
    }

    private void Uppercut()
    {
        jabCount = 0;
        animator.SetTrigger(DoUpper);
        attackState = JPPlayerAttackState.UppercutCharge;
    }

    public void UppercutLiftoff()
    {
        grounded = false;
        yVel = UppercutForce;
        attackState = JPPlayerAttackState.Uppercut;
    }
    
    // ugh what the fuck are state machines... who needs those.......
    private void UppercutLanded()
    {
        attackState = JPPlayerAttackState.Idle;
    }
    
    public override void BeginAttack()
    {
        if (!CanAttack())
            return;

        if (moveInput == Vector2.up)
        {
            Uppercut();
            return;
        }
        
        
        // Jabs
        if (jabCount == 0)
            Jab();
        else
            queuedJab = true;
    }

    public override bool HitBy(JPCharacter source, JPCharacterAttack attack)
    {
        if (!base.HitBy(source, attack)) return false;
        attackState = JPPlayerAttackState.Idle;
        jabCount = 0;
        return true;
    }


    protected new void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.darkRed;
    }
}
