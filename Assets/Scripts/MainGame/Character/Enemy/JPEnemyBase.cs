
public class JPEnemyBase : JPCharacter
{
    private JPForceInsideCamera forceInside = null;
    protected new void Start()
    {
        base.Start();
        Faction = JPCharacterFaction.Enemy;
    }

    public override bool HitBy(JPCharacter source, JPCharacterAttack attack)
    {
        forceInside ??= gameObject.AddComponent<JPForceInsideCamera>();
        
        return base.HitBy(source, attack);
    }

    protected override void Die()
    {
        if(forceInside)
            Destroy(forceInside);
        base.Die();
    }
}