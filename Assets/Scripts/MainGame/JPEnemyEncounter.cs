
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class JPEnemyPoolEntry
{
    [FormerlySerializedAs("type")] public GameObject Type;
    [FormerlySerializedAs("count")] public int Count;
    [FormerlySerializedAs("aiDirector")] public JPEnemyDirector AIDirector;
    [Range(-1,1)] public int ForceSpawnSide;
    public int MaxOnField = int.MaxValue;
    public float InternalCooldown;
    [NonSerialized] public float cooldownTimer;
    [NonSerialized] public List<JPCharacter> spawnedGuys = new();
}

public class JPEnemyEncounter : MonoBehaviour
{
    [SerializeField] private float ActivationRange;
    [SerializeField] private int MaxEnemyCount;
    [SerializeField] private float EnemySpawnCooldown;
    [SerializeField] private float MaxSpawnBounds = 999f;

    private JPCharacter player;
    private JPFollowCamera playerCamera;

    [SerializeField] public List<JPEnemyPoolEntry> EnemyPool = new();
    private List<JPCharacter> spawnedEnemies = new();

    private bool activated;
    private float enemySpawnCooldownTimer;

    private void Start()
    {
        player = FindAnyObjectByType<JPPlayerController>().GetPlayer();
        playerCamera = FindAnyObjectByType<JPFollowCamera>();
    }


    private void Update()
    {
        if (!activated)
        {
            float dist = Mathf.Abs(player.transform.position.x - transform.position.x);
            if (dist > ActivationRange)
                return;

            ForceTriggerEncounter();
        }
        else
        {
            enemySpawnCooldownTimer -= Time.deltaTime;
            spawnedEnemies.RemoveAll(e => !e || e.dead);

            foreach (JPEnemyPoolEntry jpEnemyPoolEntry in EnemyPool)
            {
                jpEnemyPoolEntry.spawnedGuys.RemoveAll(e => !e || e.dead);
                jpEnemyPoolEntry.cooldownTimer -= Time.deltaTime;
            }
            
            var canSpawn = EnemyPool.Where(t =>
                t.cooldownTimer <= 0 && t.spawnedGuys.Count < t.MaxOnField).ToArray();
            
            
            if (canSpawn.Length > 0 && spawnedEnemies.Count < MaxEnemyCount && EnemyPool.Count > 0 && enemySpawnCooldownTimer <= 0)
            {
                
                enemySpawnCooldownTimer = EnemySpawnCooldown;
                int randID = Random.Range(0, canSpawn.Length);
                JPEnemyPoolEntry entry = canSpawn[randID];
                GameObject enemyObj = Instantiate(entry.Type, transform, true);
                
                

                enemyObj.transform.position = transform.position;
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                var character = enemyObj.GetComponent<JPCharacter>();
                spawnedEnemies.Add(character);
                entry.spawnedGuys.Add(character);
                entry.AIDirector.AddManagedEnemy(character);

                bool spawnRight = entry.ForceSpawnSide switch
                {
                    -1 => false,
                    0 => Random.value < 0.5f,
                    1 => true,
                    _ => throw new ArgumentOutOfRangeException()
                };

                float rightSpawn = Mathf.Min(playerCamera.GetBounds().xMax + 3f, transform.position.x + MaxSpawnBounds);
                float leftSpawn = Mathf.Max(playerCamera.GetBounds().xMin - 3f, transform.position.x - MaxSpawnBounds);
                enemyObj.transform.position = new Vector3(
                    spawnRight ? rightSpawn : leftSpawn,
                    player.transform.position.y + 1, 
                    Random.Range(
                        JPPlayfieldZSide.bottomSide.projectedCollider.rect.GetMax().z,
                        JPPlayfieldZSide.topSide.projectedCollider.rect.GetMax().z)
                );
                

                entry.Count--;
                if (entry.Count <= 0)
                    EnemyPool.Remove(entry);
            }

            if (spawnedEnemies.Count == 0 && EnemyPool.Count == 0)
            {
                playerCamera.SetTarget(player);
                FindAnyObjectByType<JPGoSprite>().Go();
                enabled = false;
            }
        }
    }

    public void ForceTriggerEncounter(bool forceCamera = true)
    {
        activated = true;
        if(forceCamera && playerCamera.Target && playerCamera.Target.parent.TryGetComponent(out JPCharacter character))
            playerCamera.Target = transform;

        foreach (JPEnemyPoolEntry entry in EnemyPool)
        {
            entry.cooldownTimer = entry.InternalCooldown;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkGreen;
        Gizmos.DrawWireCube(transform.position, new Vector3(ActivationRange * 2, 10, 0));

        Gizmos.color = Color.darkRed;
        Gizmos.DrawWireCube(transform.position, new Vector3(MaxSpawnBounds * 2, 11, 0));
    }
}
