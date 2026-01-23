using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class JPPlayer : JPCharacter
{
    private static readonly int DoJabAnimID = Animator.StringToHash("DoJab");
    private static readonly int JabNumAnimID = Animator.StringToHash("JabNum");
    private static readonly int DoUpperAnimID = Animator.StringToHash("DoUpper");
    private static readonly int ChargingPunchAnimID = Animator.StringToHash("ChargingPunch");
    private static readonly int ChargingTimeAnimID = Animator.StringToHash("ChargingTime");


    private int jabCount;
    protected JPPlayerAttackState attackState;

    [SerializeField] private float UppercutForce;
    [SerializeField] private float UppercutForwardDir;
    [SerializeField] private JPCharacterAttack[] JabAttacks;
    [SerializeField] private JPCharacterAttack UppercutAttack;
    [SerializeField] private JPCharacterAttack ChargedPunchAttack;
    [SerializeField] private JPCharacterAttack FullyChargedPunchAttack;

    [SerializeField] private float PunchChargeTime;
    [SerializeField] private float FullChargePunchTime;
    
    private JPProjectedCollider jabHitbox;

    private float chargeTime;
    private bool holdingAttack;

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

        if (holdingAttack && attackState is JPPlayerAttackState.Idle or JPPlayerAttackState.ChargeAttack)
        {
            attackState = JPPlayerAttackState.ChargeAttack;
            chargeTime += Time.deltaTime;
            animator.SetBool(ChargingPunchAnimID, true);
        }
        else
        {
            if (attackState == JPPlayerAttackState.ChargeAttack)
                attackState = JPPlayerAttackState.Idle;
            animator.SetBool(ChargingPunchAnimID, false);
        }
        animator.SetFloat(ChargingTimeAnimID, Mathf.Clamp(chargeTime / FullChargePunchTime, .1f, 1f));
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
        animator.SetTrigger(DoJabAnimID);
        animator.SetInteger(JabNumAnimID, jabCount);
    }

    public void DoJabHitbox()
    {
        if (jabCount <= 0) return;
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
        animator.SetTrigger(DoUpperAnimID);
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

    public void ChargeAttack()
    {
        if (chargeTime < PunchChargeTime)
            return;

        JPCharacterAttack attack = chargeTime >= FullChargePunchTime ? FullyChargedPunchAttack : ChargedPunchAttack;
        
        foreach (JPProjectedCollider hurtbox in jabHitbox.CheckCollision(JPCollisionType.Hurtbox))
        {
            if(hurtbox is JPHurtableBox hurtableBox)
                hurtableBox.Hit(this, attack);
        }
    }

    public void FinishChargeAttack()
    {
        attackState = JPPlayerAttackState.Idle;
    }
    
    public override void BeginAttack()
    {
        if (!CanAttack())
            return;

        if (moveInput.y > moveInput.x && moveInput.y > 0)
        {
            Uppercut();
            return;
        }
        holdingAttack = true;
        chargeTime = 0;
        
        // Jabs
        if (jabCount == 0)
            Jab();
        else
            queuedJab = true;
    }

    public override void ReleaseAttack()
    {
        holdingAttack = false;
        
        if (!CanAttack())
            return;
    }

    public override bool HitBy(JPCharacter source, JPCharacterAttack attack)
    {
        if (!base.HitBy(source, attack)) return false;
        attackState = JPPlayerAttackState.Idle;
        jabCount = 0;
        return true;
    }
}
