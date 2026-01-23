using System;
using UnityEngine;

public class JPSeagullEnemy : JPEnemyBase, JPCustomAICharacter
{
    private static readonly int DivingAnimID = Animator.StringToHash("Diving");

    [SerializeField] private float FlyHeight;
    [SerializeField] private float RiseSpeed;
    [SerializeField] private JPCharacterAttack DiveAttack;
    
    private JPProjectedCollider attackBox;

    [NonSerialized] public int attackPhase;
    private float moveDirBeforeDive;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private new void Start()
    {
        base.Start();
        attackBox = transform.Find("AttackBox").GetComponent<JPProjectedCollider>();
    }

    private float FUCK()
    {
        attackPhase = 0;
        return FlyHeight;
    }

    protected float GetDesiredHeight()
    {
        return attackPhase switch
        {
            0 or 2 => FlyHeight,
            1 => 0,
            _ => FUCK()
        };
    }

    protected override void UpdateAnims()
    {
        base.UpdateAnims();
        animator.SetInteger(DivingAnimID, attackPhase);
    }

    protected void OhNoItIsTheDiveRocketLauncher()
    {
        foreach (JPProjectedCollider hurtbox in attackBox.CheckCollision(JPCollisionType.Hurtbox))
        {
            if (hurtbox is not JPHurtableBox hurtableBox) continue;
            if (!hurtableBox.Hit(this, DiveAttack)) continue;
            attackPhase++;
        }

        if (grounded)
            attackPhase++;
    }

    protected override Vector3 SolveVelocity()
    {
        Vector3 vel = base.SolveVelocity();

        if (damageState == JPCharacterDamageState.None && !dead)
        {
            vel.y = Mathf.Sign(GetDesiredHeight() - transform.position.y) * 
                    Mathf.Min(RiseSpeed, Mathf.Abs(GetDesiredHeight() - transform.position.y) / Time.deltaTime);
            yVel = 0;
        }

        if (attackPhase is >= 1 and <= 2)
            vel.x = MoveSpeed.x * moveDirBeforeDive;

        if (attackPhase == 2 && Mathf.Abs(GetDesiredHeight() - transform.position.y) <= .01f)
            attackPhase = 0;
        
        return vel;
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
        if (damageState != JPCharacterDamageState.None)
            attackPhase = 0;
        if (attackPhase is >= 1 and <= 2)
            OhNoItIsTheDiveRocketLauncher();
    }

    public override void BeginAttack()
    {
        if (attackPhase != 0)
            return;

        attackPhase++;
        moveDirBeforeDive = Mathf.Sign(moveInput.x);
    }

    public override bool HitBy(JPCharacter source, JPCharacterAttack attack)
    {
        if (!base.HitBy(source, attack)) return false;
        attackPhase = 0;
        return true;
    }

    public JPEnemyPuppeteer CreatePuppeteerFrom(JPCharacter target)
    {
        return new JPSeagullPuppeteer(this, target);
    }
}
