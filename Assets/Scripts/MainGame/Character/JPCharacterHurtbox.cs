
using UnityEngine;

public class JPCharacterHurtbox : JPHurtableBox
{
    [SerializeField] private JPCharacter character;

    protected new void Start()
    {
        base.Start();
        if (character == null)
        {
            character = GetComponent<JPCharacter>();
        }
    }
    
    
    public override void Hit(JPCharacter source, JPCharacterAttack attack)
    {
        character.HitBy(source, attack);
    }
}
