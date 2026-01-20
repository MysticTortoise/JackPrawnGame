using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum JPEnemyPuppeteerDirective : uint
{
    Idle,
    Standby,
    Approach
}

public class JPEnemyPuppeteer
{
    public JPCharacter enemy;
    public JPCharacter target;
    public float seeRange;

    private readonly float inRangeDist = 0.25f;
    
    public float approachDist;
    
    public float standbyDist;
    private float zStandby;
    
    
    private JPEnemyPuppeteerDirective _directive;
    public JPEnemyPuppeteerDirective directive
    {
        get => _directive;
        set
        {
            _directive = value;

            if (directive == JPEnemyPuppeteerDirective.Standby)
            {
                EnterStandby();
            }
        }
    }

    public JPEnemyPuppeteer(JPCharacter enemy, JPCharacter target)
    {
        this.enemy = enemy;
        this.target = target;
    }

    protected float GoToGoal(Vector2 targetPos)
    {
        var dir = new Vector2(targetPos.x - enemy.transform.position.x,
            targetPos.y - enemy.transform.position.z);

        float distFromGoal = dir.magnitude;

        enemy.moveInput = distFromGoal > inRangeDist ? dir.normalized : Vector2.zero;

        return distFromGoal;
    }

    protected virtual void DirectApproach()
    {
        var goal = new Vector2(
            target.transform.position.x +
            (approachDist * Mathf.Sign(enemy.transform.position.x - target.transform.position.x)),
            target.transform.position.z
        );
        if (!enemy.CombatCapable())
        {
            directive = JPEnemyPuppeteerDirective.Standby;
            return;
        }
        float dist = GoToGoal(goal);
        if (dist > inRangeDist) return;
        
        enemy.SetFacingDir(target.transform.position);
        enemy.BeginAttack();
    }

    protected virtual void EnterStandby()
    {
        zStandby = Random.Range(
            JPPlayfieldZSide.bottomSide.projectedCollider.rect.GetMax().z,
            JPPlayfieldZSide.topSide.projectedCollider.rect.GetMax().z);
    }

    protected virtual void DirectStandby()
    {
        var goal = new Vector2(
            target.transform.position.x +
            (standbyDist * Mathf.Sign(enemy.transform.position.x - target.transform.position.x)),
            zStandby
        );
        float dist = GoToGoal(goal);
        if (dist > inRangeDist) return;
        
        enemy.SetFacingDir(target.transform.position);
    }

    protected virtual void FindNewTargets()
    {
        foreach (JPCharacter character in JPCharacter.Characters
                     .Where(character => character.Faction != enemy.Faction)
                     .Where(character => (character.transform.position - enemy.transform.position).magnitude < seeRange))
        {
            target = character;
            directive = JPEnemyPuppeteerDirective.Standby;
            return;
        }
    }

    protected virtual void DirectIdle()
    {
        enemy.moveInput = Vector2.zero;
        
        FindNewTargets();
    }

    public void Direct()
    {
        // Determine target
        /*if (target is not null && (enemy.transform.position - target.transform.position).magnitude > seeRange)
        {
            target = null;
        }*/

        if (target is null)
            directive = JPEnemyPuppeteerDirective.Idle;
        
        switch (directive)
        {
            case JPEnemyPuppeteerDirective.Approach:
                DirectApproach();
                break;
            case JPEnemyPuppeteerDirective.Standby:
                DirectStandby();
                break;
            case JPEnemyPuppeteerDirective.Idle:
                DirectIdle();
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public class JPEnemyDirector : MonoBehaviour
{
    protected HashSet<JPCharacter> managing = new();
    protected HashSet<JPEnemyPuppeteer> puppeteers = new();

    [SerializeField] protected uint ToSendIn;
    [SerializeField] protected float SeeRange;
    [SerializeField] protected float ApproachDist;
    [SerializeField] protected float StandbyDist;

    private JPCharacter player;

    private void Start()
    {
        player = FindAnyObjectByType<JPPlayer>();
        managing = new HashSet<JPCharacter>(transform.GetComponentsInChildren<JPCharacter>());
    }

    private void CheckForNewPuppets()
    {
        var puppeteering = puppeteers.Select(p => p.enemy).ToHashSet();
        managing.RemoveWhere(c => !c);

        foreach (JPCharacter enemy in managing.Where(enemy => !puppeteering.Contains(enemy)))
        {
            var puppeteer = new JPEnemyPuppeteer(enemy, player)
            {
                approachDist = ApproachDist,
                seeRange = SeeRange,
                standbyDist = StandbyDist
            };
            puppeteers.Add(puppeteer);
            
        }
    }

    private void Update()
    {
        // Clear Dead Puppets
        int a = puppeteers.RemoveWhere(puppeteer => !puppeteer.enemy);
        
        CheckForNewPuppets();
        
        // Assign at least 2 guys to go after player
        int chaserCount = puppeteers.Count(p => p.directive == JPEnemyPuppeteerDirective.Approach);
        if (chaserCount < ToSendIn)
        {
            var notChasing = puppeteers.Where(p => p.directive != JPEnemyPuppeteerDirective.Approach && p.enemy.Actionable());
            foreach (JPEnemyPuppeteer puppeteer in notChasing)
            {
                if (chaserCount >= ToSendIn)
                    break;

                puppeteer.directive = JPEnemyPuppeteerDirective.Approach;
                chaserCount++;
            }
        }

        foreach (JPEnemyPuppeteer puppeteer in puppeteers)
        {
            puppeteer.Direct();
        }
    }
}
