public abstract class JPHurtableBox : JPProjectedCollider
{
    protected virtual void Start()
    {
        CollisionType = JPCollisionType.Hurtbox;
    }

    public abstract bool Hit(JPCharacter source, JPCharacterAttack attack);
}
