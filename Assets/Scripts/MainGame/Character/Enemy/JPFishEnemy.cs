
using UnityEngine;

public class JPFishEnemy : JPEnemyBase
{
    private static readonly int AttackID = Animator.StringToHash("Attack");
    private static readonly int AttackNumID = Animator.StringToHash("AttackNum");

    protected bool attacking;
    protected int jabCount;
    protected bool attackQueued;

    [SerializeField] private JPCharacterAttack[] JabAttacks;
    private JPProjectedCollider jabHitbox;
    

    protected new void Start()
    {
        base.Start();
        jabHitbox = transform.Find("JabHitbox").GetComponent<JPProjectedCollider>();
    }

    public override bool CanMove()
    {
        return base.CanMove() && !attacking;
    }

    private void Jab()
    {
        attackQueued = false;
        jabCount %= 2;
        jabCount++;
        
        animator.SetTrigger(AttackID);
        animator.SetInteger(AttackNumID, jabCount);
        attacking = true;
    }

    public void DoAttackHitbox()
    {
        if (jabCount <= 0)
            return;
        foreach (JPProjectedCollider hurtbox in jabHitbox.CheckCollision(JPCollisionType.Hurtbox))
        {
            if(hurtbox is JPHurtableBox hurtableBox)
                hurtableBox.Hit(this, JabAttacks[jabCount-1]);
        }
    }

    public override void BeginAttack()
    {
        if (!CanAttack())
            return;

        if (jabCount == 0)
            Jab();
        else
            attackQueued = true;
    }

    public override void CounterAttack()
    {
        jabCount = 1;
        Jab();
    }

    public void FinishAttack()
    {
        attacking = false;
        if (attackQueued)
            Jab();
        else
        {
            jabCount = 0;
        }
    }

    public override bool HitBy(JPCharacter source, JPCharacterAttack attack)
    {
        if (!base.HitBy(source, attack)) return false;
        attacking = false;
        jabCount = 0;
        return true;
    }
}
