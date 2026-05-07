using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] 
    private EnemyLinearMover enemy_Prefab;
    [SerializeField] 
    private Transform[] spawnPoints;
    [SerializeField] 
    private Transform enemyRoot;
    [SerializeField]
    private Transform lookAtCenter;

    [Header("Spawn Settings")]
    [SerializeField] 
    private int initialSpawnCount = 4;
    [SerializeField] 
    private int maxAliveCount = 8;
    [SerializeField] 
    private float spawnIntervalSeconds = 1f;
    [SerializeField] 
    private float enemyMoveSpeedUnitsPerSecond = 5f;
    [SerializeField] 
    private float enemyLifeTimeSeconds = 10f;

    [Header("Pool")]
    [SerializeField]
    private int defaultPoolCount = 8;
    [SerializeField] 
    private int maxPoolCount = 20;

    private Local_Object_Pool<EnemyLinearMover> enemy_Pool;
    private List<EnemyLinearMover> alive_Enemies = new List<EnemyLinearMover>();

    private float nextSpawnTimeSeconds;

    private void Awake()
    {
        enemy_Pool = new Local_Object_Pool<EnemyLinearMover>
        (
            enemy_Prefab,
            enemyRoot,
            defaultPoolCount,
            maxPoolCount
        );
    }

    private void Start()
    {
        for (int i = 0; i < initialSpawnCount; i++)
        {
            TrySpawnOneEnemy();
        }
    }

    private void Update()
    {
        RemoveDeadEntries();

        if (Time.time < nextSpawnTimeSeconds)
            return;

        if (alive_Enemies.Count >= maxAliveCount)
            return;

        if (TrySpawnOneEnemy())
        {
            nextSpawnTimeSeconds = Time.time + spawnIntervalSeconds;
        }
    }

    private bool TrySpawnOneEnemy()
    {
        if (enemy_Prefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return false;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (spawnPoint == null)
            return false;

        Quaternion rotation = spawnPoint.rotation;

        if (lookAtCenter != null)
        {
            Vector3 toCenter = lookAtCenter.position - spawnPoint.position;

            if (toCenter.sqrMagnitude > 0.001f)
            {
                rotation = Quaternion.LookRotation(toCenter.normalized, Vector3.up);
            }
        }

        EnemyLinearMover enemy = enemy_Pool.Get();

        enemy.transform.position = spawnPoint.position;
        enemy.transform.rotation = rotation;

        enemy.Set_Pool(this);

        enemy.Initialize
        (
            enemyMoveSpeedUnitsPerSecond,
            enemyLifeTimeSeconds
        );

        alive_Enemies.Add(enemy);

        return true;
    }

    public void Return_Enemy(EnemyLinearMover enemy)
    {
        if (enemy == null)
            return;

        alive_Enemies.Remove(enemy);

        enemy_Pool.Return(enemy);
    }

    private void RemoveDeadEntries()
    {
        for (int i = alive_Enemies.Count - 1; i >= 0; i--)
        {
            if (alive_Enemies[i] == null)
            {
                alive_Enemies.RemoveAt(i);
            }
        }
    }
}