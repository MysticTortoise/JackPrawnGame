public enum JPPlayerAttackState
{
    Idle,
    Jab1,
    Jab2,
    Jab3,
    Uppercut,
    UppercutCharge,
    ChargeAttack
}

public static class JPPlayerStateExtensions
{
    public static bool CanAttack(this JPPlayerAttackState attackState)
    {
        return attackState switch
        {
            JPPlayerAttackState.Idle 
                or JPPlayerAttackState.Jab1
                or JPPlayerAttackState.Jab2
                or JPPlayerAttackState.Jab3
                => true,
            _ => false
        };
    }
    
    public static bool CanMove(this JPPlayerAttackState attackState)
    {
        return !IsAttacking(attackState);
    }

    public static bool IsAttacking(this JPPlayerAttackState attackState)
    {
        return attackState != JPPlayerAttackState.Idle;
    }
}