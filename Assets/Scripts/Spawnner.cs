using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Renderer floor;
    [SerializeField] int count = 10;
    [SerializeField] float margin = 3f;
    [SerializeField] float height = 1.5f;
    [SerializeField] float clearRadius = 1.5f;
    [SerializeField] float spawnInterval = 1.0f; // Time in seconds between spawns

    void Start()
    {
        StartCoroutine(SpawnCoinsRoutine());
    }

    IEnumerator SpawnCoinsRoutine()
    {
        Bounds b = floor.bounds;
        int placed = 0, tries = 0;

        while (placed < count && tries < 500)
        {
            tries++;
            float x = Random.Range(b.min.x + margin, b.max.x - margin);
            float z = Random.Range(b.min.z + margin, b.max.z - margin);

            Vector3 probe = new Vector3(x, b.max.y + clearRadius + 0.05f, z);
            if (Physics.CheckSphere(probe, clearRadius, ~0, QueryTriggerInteraction.Collide))
                continue;

            Vector3 at = new Vector3(x, b.max.y + height, z);
            Instantiate(coinPrefab, at, coinPrefab.transform.rotation);
            placed++;

            // Wait before spawning the next coin
            yield return new WaitForSeconds(spawnInterval);
        }

        if (placed < count)
            Debug.LogWarning($"CoinSpawner placed only {placed}/{count}");
    }
}