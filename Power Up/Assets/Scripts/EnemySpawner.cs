using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    private void OnEnable()
    {
        LevelNavmesh.OnNavMeshBuilt += SpawnEnemy;
    }
    private void OnDisable()
    {
        LevelNavmesh.OnNavMeshBuilt -= SpawnEnemy;
    }
    void SpawnEnemy ()
    {
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(transform.position, out closestHit, 10, 1))
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.position = closestHit.position;
            enemy.GetComponent<EnemyAI>().navMeshAgent = enemy.AddComponent<NavMeshAgent>();
        }
    }
}
