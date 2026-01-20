public abstract class JPHurtableBox : JPProjectedCollider
{
    protected void Start()
    {
        CollisionType = JPCollisionType.Hurtbox;
    }

    public abstract void Hit(JPCharacter source, JPCharacterAttack attack);
}
