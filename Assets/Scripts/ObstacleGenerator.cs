using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Generates and spawns obstacles in the game world.
/// </summary>
public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] private float xLimit = 200f; 
    [SerializeField] private float zLimit = 1000f; 
    [SerializeField] private List<Transform> obstaclePrefabs = new List<Transform>(); // List of obstacle prefabs to spawn
    [SerializeField] private LayerMask obstacleLayer; // Layer mask for obstacle collision detection
    [SerializeField] private float checkRadius = 38f; // Radius to check for obstacles before spawning
    public float minDistanceBetweenObstacles = 10f; 

    public int totalObstacles = 500; 
    private const int maxSpawnAttempts = 100; 

    private void Start()
    {
        SpawnObstacles(); // Start spawning obstacles when the script is initialized
    }

    private void SpawnObstacles()
    {
        int obstaclesSpawned = 0; 
        List<Vector3> obstaclePositions = new List<Vector3>(); // List to store the positions of spawned obstacles

        // Keep spawning obstacles until the desired number is reached
        while (obstaclesSpawned < totalObstacles)
        {
            int spawnAttempt = 0; 
            bool spawnSuccessful = false; // Flag to track successful spawns

            // Try to spawn an obstacle up to the maximum spawn attempts
            while (spawnAttempt < maxSpawnAttempts)
            {
                Vector3 randomPos = GetRandomPoint(); 

                // Check if the new position is far enough from existing obstacles
                bool isFarEnough = true;
                foreach (Vector3 obstaclePos in obstaclePositions)
                {
                    if (Vector3.Distance(randomPos, obstaclePos) < minDistanceBetweenObstacles)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                // Check if the new position is spawnable
                if (isFarEnough && IsSpawnable(randomPos))
                {
                    Transform prefabToSpawn = GetRandomObstacle();
                    Vector3 randomScale = GetRandomScale();

                    Transform spawnedObstacle = Instantiate(prefabToSpawn, randomPos, Quaternion.identity);
                    spawnedObstacle.localScale = randomScale;
                    obstaclesSpawned++; 
                    spawnSuccessful = true;

                    // Store the position of the spawned obstacle
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
        return obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)]; // Return a random obstacle prefab from the list
    }

    private bool IsSpawnable(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, checkRadius, obstacleLayer); // Check for colliders within the given radius
        return colliders.Length == 0; 
    }

    private Vector3 GetRandomPoint()
    {
        // Generate a random position within the specified limits
        return new Vector3(Random.Range(-xLimit, xLimit), Random.Range(-2f, 6f), Random.Range(3f, zLimit));
    }

    private Vector3 GetRandomScale()
    {
        float scaleMultiplierX = Random.Range(10f, 20f); // Random scale multiplier for x and y-axis
        float scaleMultiplier = Random.Range(5f, 10f); 
        return new Vector3(scaleMultiplierX, scaleMultiplier, scaleMultiplierX); // Return a random scale vector
    }
}
