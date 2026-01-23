
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
    
    
    public override bool Hit(JPCharacter source, JPCharacterAttack attack)
    {
        return character.HitBy(source, attack);
    }
}
