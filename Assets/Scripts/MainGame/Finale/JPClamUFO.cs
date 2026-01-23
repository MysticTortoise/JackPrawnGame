
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public enum JPClamUFOState
{
    Moving,
    Down,
    Rising
}

public class JPClamUFO : JPHurtableBox
{
    [FormerlySerializedAs("leftDist")] [SerializeField] private float LeftDist;
    [FormerlySerializedAs("MoveSpeed")] [SerializeField] private float BounceSpeed;
    [SerializeField] private float UfoTurnAmount;
    [SerializeField] private float XMoveSpeed;
    
    [SerializeField] private float FlyHeight;
    [SerializeField] private float RiseSpeed;

    [SerializeField] private float AttackInterval;
    [SerializeField] private GameObject Fireball;

    [NonSerialized] public JPClamUFOState state = JPClamUFOState.Rising;

    private float leftPos;
    private float rightPos;
    private float posAlpha;
    private float posTimer;

    private float ufoRotGoal;
    private float ufoRot;

    private float attackTimer;

    private Transform visual;

    private JPEnemyEncounter encounter;
    private JPCharacter player;

    private JPCharacter dummyCharacter;

    private JPClamButton[] buttons;

    private int health = 3;

    private void Start()
    {
        base.Start();
        rightPos = transform.position.x;
        leftPos = rightPos - LeftDist;

        visual = transform.Find("Visual");
        player = FindAnyObjectByType<JPPlayerController>().GetPlayer();

        buttons = FindObjectsByType<JPClamButton>(FindObjectsSortMode.None);

    }

    private void Die()
    {
        SceneManager.LoadScene("EndScene");
    }

    public override bool Hit(JPCharacter source, JPCharacterAttack attack)
    {
        if (source.Faction != JPCharacterFaction.Player) return false;

        if (state != JPClamUFOState.Down) return false;
        
        state = JPClamUFOState.Rising;
        health--;

        foreach (JPClamButton button in buttons)
            button.hit = false;

        if (health <= 0)
        {
            Die();
        }
        return true;

    }

    private void OnEnable()
    {
        base.OnEnable();
        encounter = FindAnyObjectByType<JPEnemyEncounter>();
        encounter.ForceTriggerEncounter(false);

        dummyCharacter = transform.Find("DummyChar").GetComponent<JPCharacter>();
        StartCoroutine(DoHPBar());
    }

    private IEnumerator DoHPBar()
    {
        yield return new WaitForSeconds(1);
        FindObjectsByType<JPHealthDisplay>(FindObjectsSortMode.None).First(d => !d.Target).Target = dummyCharacter;
    }

    private void CheckForButtons()
    {
        if (buttons.Any(button => !button.hit))
            return;

        state = JPClamUFOState.Down;
    }

    private void Update()
    {
        encounter.EnemyPool[0].ForceSpawnSide = (int)-Mathf.Sign(player.transform.position.x - encounter.transform.position.x);
        dummyCharacter.currentHealth = health;
        
        CheckForButtons();
        
        switch (state)
        {
            case JPClamUFOState.Moving:
                MoveTick();
                break;
            case JPClamUFOState.Rising:
                RisingTick();
                break;
            case JPClamUFOState.Down:
                DownTick();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ufoRot = JPMath.Damp(ufoRot, ufoRotGoal, 3, Time.deltaTime);
        visual.rotation = Quaternion.Euler(0, 0, ufoRot);
    }

    /*
    private void AttackingTick()
    {
        attackTimer += Time.deltaTime;
        ufoRotGoal = 0;

        if (attackTimer > TimeBeforeAttacking)
        {
            // do attack
        }

        if (attackTimer > TimeBeforeAttacking + TimeAfterAttacking)
        {
            state = JPClamUFOState.Moving;
            attackTimer = 0;
            return;
        }
    }*/

    private void DownTick()
    {
        Vector3 pos = transform.position;
        
        // Rise/lower
        float amnt = Mathf.Sign(0 - transform.position.y) * 
                     Mathf.Min(RiseSpeed * Time.deltaTime, Mathf.Abs(0 - transform.position.y));

        pos.y += amnt;
        ufoRotGoal = 0;
        // Apply
        transform.position = pos;
    }

    private void RisingTick()
    {
        Vector3 pos = transform.position;
        
        // Rise/lower
        float amnt = Mathf.Sign(FlyHeight - transform.position.y) * 
                 Mathf.Min(RiseSpeed * Time.deltaTime, Mathf.Abs(FlyHeight - transform.position.y));

        pos.y += amnt;
        if (Mathf.Approximately(amnt, 0))
        {
            state = JPClamUFOState.Moving;
        }
        
        // Apply
        transform.position = pos;
    }


    private void MoveTick()
    {
        Vector3 pos = transform.position;
        
        // Rise/lower
        pos.y += Mathf.Sign(FlyHeight - transform.position.y) * 
                         Mathf.Min(RiseSpeed * Time.deltaTime, Mathf.Abs(FlyHeight - transform.position.y));
        
        // Timer management
        posTimer += Time.deltaTime * BounceSpeed;
        posAlpha = (Mathf.Sin(posTimer) + 1) / 2;

        // Left/Right movement
        float goalX = Mathf.Lerp(rightPos, leftPos, posAlpha);
        
        pos.x += Mathf.Sign(goalX - transform.position.x) * 
                 Mathf.Min(XMoveSpeed * Time.deltaTime, Mathf.Abs(goalX - transform.position.x));
        
        ufoRotGoal = Mathf.Cos(posTimer + 0.5f) * UfoTurnAmount;

        attackTimer += Time.deltaTime;
        if (attackTimer >= AttackInterval)
        {
            attackTimer = 0;
            GameObject obj = Instantiate(Fireball);
            obj.transform.position = transform.position;
        }

        // Apply
        transform.position = pos;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.left * LeftDist);
    }
    #endif
}
