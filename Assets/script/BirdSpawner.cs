using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject[] birdPrefabs; // Array of bird prefabs to spawn
    public Transform[] waypoints; // Waypoints for the birds to follow
    public int numberOfBirds = 3; // Number of birds to spawn
    public float spawnInterval = 2.0f; // Time between spawns

    void Start()
    {
        // Start spawning birds
        for (int i = 0; i < numberOfBirds; i++)
        {
            Invoke("SpawnBird", i * spawnInterval);
        }
    }

    void SpawnBird()
    {
        // Check if there are any bird prefabs in the array
        if (birdPrefabs.Length == 0)
        {
            Debug.LogWarning("No bird prefabs assigned!");
            return;
        }

        // Randomly select a bird prefab from the array
        int randomIndex = Random.Range(0, birdPrefabs.Length);
        GameObject birdPrefab = birdPrefabs[randomIndex];

        // Instantiate the selected bird prefab at the first waypoint
        GameObject bird = Instantiate(birdPrefab, waypoints[0].position, Quaternion.identity);

        // Assign waypoints to the bird
        BirdMovement birdMovement = bird.GetComponent<BirdMovement>();
        if (birdMovement != null)
        {
            birdMovement.waypoints = waypoints;
        }
    }
}