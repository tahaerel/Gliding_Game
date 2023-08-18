using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] private float xLimit = 200f;
    [SerializeField] private float zLimit = 1000f;
    [SerializeField] private List<Transform> obstaclePrefabs = new List<Transform>();
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float checkRadius = 38f;
    public float minDistanceBetweenObstacles = 10f; // Minimum uzaklýk

    public int totalObstacles = 500;
    private const int maxSpawnAttempts = 100;

    private void Start()
    {
        SpawnObstacles();
    }

    private void SpawnObstacles()
    {
        int obstaclesSpawned = 0;
        List<Vector3> obstaclePositions = new List<Vector3>(); // Saklanan pozisyonlar

        while (obstaclesSpawned < totalObstacles)
        {
            int spawnAttempt = 0;
            bool spawnSuccessful = false;

            while (spawnAttempt < maxSpawnAttempts)
            {
                Vector3 randomPos = GetRandomPoint();

                // Yeni pozisyonun daha önce spawn edilen nesnelerle minimum uzaklýkta olup olmadýðýný kontrol et
                bool isFarEnough = true;
                foreach (Vector3 obstaclePos in obstaclePositions)
                {
                    if (Vector3.Distance(randomPos, obstaclePos) < minDistanceBetweenObstacles)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                if (isFarEnough && IsSpawnable(randomPos))
                {
                    Transform prefabToSpawn = GetRandomObstacle();
                    Vector3 randomScale = GetRandomScale();

                    Transform spawnedObstacle = Instantiate(prefabToSpawn, randomPos, Quaternion.identity);
                    spawnedObstacle.localScale = randomScale;
                    obstaclesSpawned++;
                    spawnSuccessful = true;

                    // Spawn edilen nesnenin pozisyonunu sakla
                    obstaclePositions.Add(randomPos);

                    break;
                }

                spawnAttempt++;
            }

            if (!spawnSuccessful)
            {
                Debug.LogWarning("Max spawn attempts reached. Stopping spawning.");
                break;
            }
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
        return new Vector3(Random.Range(-xLimit, xLimit), Random.Range(-2f, 6f), Random.Range(3f, zLimit));
    }

    private Vector3 GetRandomScale()
    {
        float scaleMultiplierx = Random.Range(10f, 20f);
        float scaleMultiplier = Random.Range(5f, 10f); // Adjust the range as desired
        return new Vector3(scaleMultiplierx, scaleMultiplier, scaleMultiplierx);
    }
}
