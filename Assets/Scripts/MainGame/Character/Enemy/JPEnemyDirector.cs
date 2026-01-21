using System;
using System.Collections.Generic;
using System.Linq;
using JPDebugDraw;
using UnityEngine;
using Random = UnityEngine.Random;

public enum JPEnemyPuppeteerDirective : uint
{
    Idle,
    Standby,
    Approach,
}

public class JPEnemyPuppeteer
{
    public JPCharacter enemy;
    public JPCharacter target;
    public float seeRange;

    private readonly float inRangeDist = 0.25f;
    
    public float approachDist;
    public bool approachRight;
    
    public float standbyDist;
    private float zStandby;

    public float goAroundDist;
    private bool underOrOver;
    
    
    private JPEnemyPuppeteerDirective _directive;
    public JPEnemyPuppeteerDirective directive
    {
        get => _directive;
        set
        {
            _directive = value;

            switch (directive)
            {
                case JPEnemyPuppeteerDirective.Standby:
                    EnterStandby();
                    break;
                case JPEnemyPuppeteerDirective.Idle:
                case JPEnemyPuppeteerDirective.Approach:
                default: break;
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
        
        JPDebugDrawer.AddCommand(new JPLineDrawCommand(enemy.transform.position, new Vector3(targetPos.x, 0, targetPos.y), Color.brown));

        return distFromGoal;
    }

    protected float GetDistFromGoal(Vector2 targetPos)
    {
        return (new Vector2(targetPos.x - enemy.transform.position.x,
            targetPos.y - enemy.transform.position.z)).magnitude;
    }

    protected virtual void DirectApproach()
    {
        if (!enemy.CombatCapable())
        {
            directive = JPEnemyPuppeteerDirective.Standby;
            return;
        }
        
        var approachGoal = new Vector2(
            target.transform.position.x +
            approachDist * (approachRight ? 1 : -1),
            target.transform.position.z
        );
        
        var aroundGoal = new Vector2(
            target.transform.position.x,
            target.transform.position.z + (goAroundDist * (underOrOver ? 1 : -1))
        );
        
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        bool shouldGoAround = Mathf.Sign(approachGoal.x - target.transform.position.x) != Mathf.Sign(enemy.transform.position.x - target.transform.position.x);

        float distFromAround = GetDistFromGoal(aroundGoal);

        shouldGoAround &= distFromAround > approachDist * 2f;
        if (shouldGoAround)
        {
            GoToGoal(aroundGoal);
            return;
        }

        float dist = GoToGoal(approachGoal);
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
    [SerializeField] protected uint ToFlank;
    [SerializeField] protected float SeeRange;
    [SerializeField] protected float ApproachDist;
    [SerializeField] protected float StandbyDist;
    [SerializeField] protected float GoAroundDist;

    private JPCharacter player;

    private void Start()
    {
        player = FindAnyObjectByType<JPPlayer>();
        managing = new HashSet<JPCharacter>(transform.GetComponentsInChildren<JPCharacter>());
    }

    private void CheckForNewPuppets()
    {
        var puppeteering = puppeteers .Select(p => p.enemy).ToHashSet();
        managing.RemoveWhere(c => !c || c.dead);

        foreach (JPCharacter enemy in managing.Where(enemy => !puppeteering.Contains(enemy)))
        {
            var puppeteer = new JPEnemyPuppeteer(enemy, player)
            {
                approachDist = ApproachDist,
                seeRange = SeeRange,
                standbyDist = StandbyDist,
                goAroundDist = GoAroundDist
            };
            puppeteers.Add(puppeteer);
            
        }
    }

    private void Update()
    {
        // Clear Dead Puppets
        int a = puppeteers.RemoveWhere(puppeteer => !puppeteer.enemy || puppeteer.enemy.dead);
        
        CheckForNewPuppets();

        var approachers = puppeteers.Where(p => p.directive == JPEnemyPuppeteerDirective.Approach).ToArray();
        int righties = approachers.Count(p => p.approachRight);
        int lefties = approachers.Count(p => !p.approachRight);

        if (righties > lefties)
        {
            // righties is confronter
            if (righties < ToSendIn)
            {
                foreach (JPEnemyPuppeteer puppeteer in puppeteers.Where(p => p.directive == JPEnemyPuppeteerDirective.Standby && p.enemy.CombatCapable()).OrderBy(p => -p.enemy.transform.position.x))
                {
                    puppeteer.directive = JPEnemyPuppeteerDirective.Approach;
                    puppeteer.approachRight = true;
                    righties++;
                    if (righties >= ToSendIn)
                        break;
                }
            }
            else if(righties > ToSendIn)
            {
                foreach (JPEnemyPuppeteer puppeteer in approachers.Where(p => p.approachRight))
                {
                    puppeteer.directive = JPEnemyPuppeteerDirective.Standby;
                    righties--;
                    if (righties <= ToSendIn)
                        break;
                }
            }

            if (lefties < ToFlank)
            {
                foreach (JPEnemyPuppeteer puppeteer in puppeteers.Where(p => p.directive == JPEnemyPuppeteerDirective.Standby && p.enemy.CombatCapable()).OrderBy(p => p.enemy.transform.position.x))
                {
                    puppeteer.directive = JPEnemyPuppeteerDirective.Approach;
                    puppeteer.approachRight = false;
                    lefties++;
                    if (lefties >= ToFlank)
                        break;
                }
            } if(lefties > ToFlank)
            {
                foreach (JPEnemyPuppeteer puppeteer in approachers.Where(p => !p.approachRight))
                {
                    puppeteer.directive = JPEnemyPuppeteerDirective.Standby;
                    lefties--;
                    if (lefties <= ToFlank)
                        break;
                }
            }
        }
        else
        {
            // lefties is confronter
            if (lefties < ToSendIn)
            {
                foreach (JPEnemyPuppeteer puppeteer in puppeteers.Where(p => p.directive == JPEnemyPuppeteerDirective.Standby && p.enemy.CombatCapable()).OrderBy(p => p.enemy.transform.position.x))
                {
                    puppeteer.directive = JPEnemyPuppeteerDirective.Approach;
                    puppeteer.approachRight = true;
                    lefties++;
                    if (lefties >= ToSendIn)
                        break;
                }
            } if(lefties > ToSendIn)
            {
                foreach (JPEnemyPuppeteer puppeteer in approachers.Where(p => !p.approachRight))
                {
                    puppeteer.directive = JPEnemyPuppeteerDirective.Standby;
                    lefties--;
                    if (lefties <= ToSendIn)
                        break;
                }
            }

            if (righties < ToFlank)
            {
                foreach (JPEnemyPuppeteer puppeteer in puppeteers.Where(p => p.directive == JPEnemyPuppeteerDirective.Standby && p.enemy.CombatCapable()).OrderBy(p => -p.enemy.transform.position.x))
                {
                    puppeteer.directive = JPEnemyPuppeteerDirective.Approach;
                    puppeteer.approachRight = false;
                    righties++;
                    if (righties >= ToFlank)
                        break;
                }
            }
            else if (righties > ToFlank)
            {
                foreach (JPEnemyPuppeteer puppeteer in approachers.Where(p => p.approachRight))
                {
                    puppeteer.directive = JPEnemyPuppeteerDirective.Standby;
                    righties--;
                    if (righties <= ToFlank)
                        break;
                }
            }
        }

        puppeteers.RemoveWhere(p => !p.enemy);
        
        foreach (JPEnemyPuppeteer puppeteer in puppeteers)
        {
            puppeteer.Direct();
        }
    }

    public void AddManagedEnemy(JPCharacter character)
    {
        managing.Add(character);
    }
}
