using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator: MonoBehaviour
{
    [SerializeField] private float xLimit = 200f;
    [SerializeField] private float zLimit = 1000f;
    [SerializeField] private List<Transform> obstaclePrefabs = new List<Transform>();
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float checkRadius = 38f;

    private const int totalObstacles = 300;
    private const int maxSpawnAttempts =100;

    private void Start()
    {
        SpawnObstacles();
    }

    private void SpawnObstacles()
    {
        int obstaclesSpawned = 0;

        while (obstaclesSpawned < totalObstacles)
        {
            int spawnAttempt = 0;
            Vector3 randomPos = GetRandomPoint();

            if (IsSpawnable(randomPos))
            {
                Transform prefabToSpawn = GetRandomObstacle();
                Vector3 randomScale = GetRandomScale();
                Transform spawnedObstacle = Instantiate(prefabToSpawn, randomPos, Quaternion.identity);
                spawnedObstacle.localScale = randomScale;
                obstaclesSpawned++;
            }

            if (spawnAttempt >= maxSpawnAttempts)
            {
                Debug.LogWarning("Max spawn attempts reached. Stopping spawning.");
                break;
            }

            spawnAttempt++;
        }
    }

    private Transform GetRandomObstacle()
    {
        return obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
    }

    private bool IsSpawnable(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, checkRadius, obstacleLayer);
        return colliders.Length == 0;
    }

    private Vector3 GetRandomPoint()
    {
        return new Vector3(Random.Range(-xLimit, xLimit), Random.Range(-2f, 6f), Random.Range(3, zLimit));
    }

    private Vector3 GetRandomScale()
    {
        float scaleMultiplier = Random.Range(5f, 12f); // Adjust the range as desired
        return new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
    }
}
