
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
class JPEnemyPoolEntry
{
    public GameObject type;
    public int count;
    public JPEnemyDirector aiDirector;
}

public class JPEnemyEncounter : MonoBehaviour
{
    [SerializeField] private float ActivationRange;
    [SerializeField] private int MaxEnemyCount;
    [SerializeField] private float EnemySpawnCooldown;

    private JPCharacter player;
    private JPFollowCamera playerCamera;

    [SerializeField] private List<JPEnemyPoolEntry> EnemyPool = new();
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

            activated = true;
            playerCamera.Target = transform;
        }
        else
        {
            spawnedEnemies.RemoveAll(e => e.dead);
            enemySpawnCooldownTimer -= Time.deltaTime;
            if (spawnedEnemies.Count < MaxEnemyCount && EnemyPool.Count > 0 && enemySpawnCooldownTimer <= 0)
            {
                enemySpawnCooldownTimer = EnemySpawnCooldown;
                int randID = Random.Range(0, EnemyPool.Count);
                JPEnemyPoolEntry entry = EnemyPool[randID];
                GameObject enemyObj = Instantiate(entry.type, transform, true);
                
                

                enemyObj.transform.position = transform.position;
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                var character = enemyObj.GetComponent<JPCharacter>();
                spawnedEnemies.Add(character);
                entry.aiDirector.AddManagedEnemy(character);
                
                enemyObj.transform.position = new Vector3(
                    Random.value < 0.5f ? playerCamera.GetBounds().xMin - 3f : playerCamera.GetBounds().xMax + 3f,
                    player.transform.position.y + 1, 
                    Random.Range(
                        JPPlayfieldZSide.bottomSide.projectedCollider.rect.GetMax().z,
                        JPPlayfieldZSide.topSide.projectedCollider.rect.GetMax().z)
                );
                

                entry.count--;
                if (entry.count <= 0)
                    EnemyPool.RemoveAt(randID);
            }

            if (spawnedEnemies.Count == 0 && EnemyPool.Count == 0)
            {
                playerCamera.SetTarget(player);
                this.enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkGreen;
        Gizmos.DrawWireCube(transform.position, new Vector3(ActivationRange * 2, 10, 0));
    }
}
