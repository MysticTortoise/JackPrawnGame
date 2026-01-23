
using System;
using UnityEngine;

public class JPClamUFOProjectile  : MonoBehaviour
{
    [SerializeField] private float DownSpeed;
    [SerializeField] private JPCharacterAttack Attack;
    
    private JPProjectedCollider hitbox;
    private JPCharacter dummyCharacter;
    private void Start()
    {
        hitbox = GetComponent<JPProjectedCollider>();
        dummyCharacter = transform.GetChild(0).GetComponent<JPCharacter>();
    }

    private void Update()
    {
        transform.position += Vector3.down * (DownSpeed * Time.deltaTime);
        foreach (JPProjectedCollider hurtbox in hitbox.CheckCollision(JPCollisionType.Hurtbox | JPCollisionType.Solid))
        {
            if (hurtbox is JPHurtableBox hurtableBox)
            {
                hurtableBox.Hit(dummyCharacter, Attack);
                Destroy(gameObject);
            }
            else if ((hurtbox.CollisionType & JPCollisionType.Solid) != 0)
            {
                var fx = Instantiate(Attack.AttackEffect);
                fx.transform.position = transform.GetChild(0).position;
                Destroy(gameObject);
            }
        }
    }
}
