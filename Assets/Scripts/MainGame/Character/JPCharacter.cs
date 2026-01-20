using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class JPCharacter : MonoBehaviour
{
    private static readonly int MovingAnimID = Animator.StringToHash("Moving");
    private static readonly int GroundedAnimID = Animator.StringToHash("Grounded");
    private static readonly int HurtID = Animator.StringToHash("Hurt");
    private static readonly int DeadAnimID = Animator.StringToHash("Die");
    private static readonly int DamageStateID = Animator.StringToHash("DamageState");

    public static HashSet<JPCharacter> Characters = new();

    // Parameters
    [FormerlySerializedAs("moveSpeed")] [SerializeField] protected Vector2 MoveSpeed;

    [FormerlySerializedAs("gravityForce")] [SerializeField] private float GravityForce;

    [SerializeField] protected float FlinchForce;
    [SerializeField] protected float FlinchTime;
    [SerializeField] protected float FlyingForce;
    [SerializeField] protected float BounceDetract;
    [SerializeField] protected float FlinchMoveRecoverSpeed = 1;
    [SerializeField] protected float DeathForce = 20;
    
    [DoNotSerialize] public JPCharacterFaction Faction;
    
    // Components
    private JPProjectedCollider footCollider;
    protected Transform visualHandler;
    protected Animator animator;
    
    // State
    [NonSerialized] public Vector2 moveInput;
    protected JPCharacterDamageState damageState = JPCharacterDamageState.None;
    public int Health;
    
    [NonSerialized] public Vector3 flinchMove = Vector3.zero;
    protected float flinchTimer;
    
    protected float yVel;
    protected bool grounded;

    protected bool facingDir;
    protected bool dead;

    private void SetupComponents()
    {
        footCollider = GetComponents<JPProjectedCollider>()
            .First(col => (col.CollisionType & JPCollisionType.Hurtbox) == 0);
        visualHandler = transform.Find("Visuals");
        animator = visualHandler.GetComponent<Animator>();
    }

    public virtual bool CanMove()
    {
        return Actionable();
    }

    public virtual bool Actionable()
    {
        return damageState == JPCharacterDamageState.None && !dead;
    }

    public virtual bool CombatCapable()
    {
        switch (damageState)
        {
            case JPCharacterDamageState.Flying:
            case JPCharacterDamageState.Stunned:
            case JPCharacterDamageState.Getup:
                return false;
        }

        return !dead;
    }

    public virtual bool CanAttack()
    {
        return Actionable();
    }

    protected void OnEnable()
    {
        Characters.Add(this);
    }

    protected void OnDisable()
    {
        Characters.Remove(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        SetupComponents();
    }

    public void SetFacingDir(bool newFacingDir)
    {
        facingDir = newFacingDir;
        visualHandler.localScale = newFacingDir ? Vector3.one : new Vector3(-1, 1, 1);
    }

    public void SetFacingDir(Vector3 position)
    {
        SetFacingDir(transform.position.x < position.x);
    }

    protected virtual Vector3 SolveVelocity()
    {
        // Movement input
        Vector3 velocity = Vector3.zero;

        // Movement
        if (CanMove())
        {
            velocity += new Vector3(
                moveInput.x * MoveSpeed.x, 0, moveInput.y * MoveSpeed.y);
        }

        velocity += flinchMove;

        // Gravity
        if (grounded && yVel <= 0)
        {
            if (damageState == JPCharacterDamageState.Flying)
            {
                yVel = -yVel / BounceDetract;
                if (yVel <= 1)
                {
                    damageState = JPCharacterDamageState.None;
                    yVel = 0;
                    flinchTimer = 0;
                }
            }
            else
            {
                yVel = 0;
            }
        }
        else
            yVel -= GravityForce * Time.deltaTime;
        
        velocity.y += yVel;

        return velocity;
    }

    protected virtual void UpdateAnims()
    {
        animator.SetBool(MovingAnimID, CanMove() && moveInput.magnitude > 0);

        if (CanMove() && moveInput.x != 0)
        {
            SetFacingDir(moveInput.x > 0);
        }
        
        animator.SetBool(GroundedAnimID, grounded);
        animator.SetBool(HurtID, damageState != JPCharacterDamageState.None);
        animator.SetInteger(DamageStateID,(int)damageState);
    }
    

    // Update is called once per frame
    protected virtual void Update()
    {
        // Apply velocity
        footCollider.MoveCollider(SolveVelocity() * Time.deltaTime);
        
        // Check for floor.
        grounded = JPProjectedCollider
            .GetCollidersIn(GetFloorCheckRect(), JPCollisionType.Solid)
            .Count(solidCollider => solidCollider != footCollider) > 0 && yVel <= 0;
        
        // Flinch Time
        flinchTimer -= Time.deltaTime;
        if (flinchTimer <= 0)
        {
            flinchMove = Vector3.zero;
            flinchTimer = 0;
            damageState = JPCharacterDamageState.None;
        }

        if (grounded)
        {
            flinchMove -= new Vector3(
                Mathf.Clamp(flinchMove.x, -FlinchMoveRecoverSpeed, FlinchMoveRecoverSpeed), 
                Mathf.Clamp(flinchMove.y, -FlinchMoveRecoverSpeed, FlinchMoveRecoverSpeed), 
                Mathf.Clamp(flinchMove.z, -FlinchMoveRecoverSpeed, FlinchMoveRecoverSpeed));
        }
        
        UpdateAnims();
    }

    private JPRect GetFloorCheckRect()
    {
        JPRect floorCheckRect = footCollider.rect;
        floorCheckRect.Position.y -= 0.01f;
        floorCheckRect.Extents -= new Vector3(0.01f, 0, 0.01f);
        return floorCheckRect;
    }
    
    public virtual void BeginAttack()
    {
        
    }

    public virtual void ReleaseAttack()
    {
        
    }

    protected virtual bool CanBeHit()
    {
        return damageState != JPCharacterDamageState.Flying && !dead;
    }

    public virtual bool HitBy(JPCharacter source, JPCharacterAttack attack)
    {
        if ((source.Faction & Faction) != 0)
            return false;

        if (!CanBeHit())
            return false;
        
        Health -= attack.Damage;
        if(Health <= 0)
            Die();

        SetFacingDir(transform.position.x - source.transform.position.x < 0);
        
        if (attack.DamageSeverity < damageState)
            return true;

        if (damageState == JPCharacterDamageState.Stunned && attack.DamageSeverity == JPCharacterDamageState.Stunned)
        {
            damageState = JPCharacterDamageState.None;
            flinchTimer = 0;
            flinchMove = Vector3.zero;
            BeginAttack();
            return false;
        }
        
        damageState = attack.DamageSeverity;

        float sign = Mathf.Sign(transform.position.x - source.transform.position.x);

        switch (attack.DamageSeverity)
        {
            case JPCharacterDamageState.Flinch:
                flinchMove = Vector3.right * sign * FlinchForce * attack.EffectStrength;
                flinchTimer = FlinchTime;
                break;
            case JPCharacterDamageState.Flying:
                flinchMove = Vector3.right * sign * FlyingForce * (attack.EffectStrength / 30);
                yVel = attack.EffectStrength;
                flinchTimer = 999;
                break;
            case JPCharacterDamageState.Stunned:
                flinchTimer = attack.EffectStrength;
                grounded = false;
                break;
            case JPCharacterDamageState.None:
            case JPCharacterDamageState.Getup:
            default:
                throw new InvalidEnumArgumentException();
        }

        return true;
    }

    protected virtual void Die()
    {
        dead = true;
        yVel += DeathForce;
        animator.SetTrigger(DeadAnimID);
    }
    

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.lavender;
        
        SetupComponents();

        JPRect floorCheckRect = GetFloorCheckRect();
        Gizmos.DrawWireCube(floorCheckRect.Position, floorCheckRect.Extents * 2);
    }

    public virtual void DoneDying()
    {
        Destroy(gameObject);
    }
}
