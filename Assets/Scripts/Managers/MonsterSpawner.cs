using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;
    public int monsterCount = 3;
    public bool enableDebugLogs = true;
    public float spawnRadius = 2f; // Increased from 0.5f to 2f for more spread
    public float minHeight = 0f; // Minimum Y offset from spawner
    public float maxHeight = 0f; // Maximum Y offset from spawner

    void Start()
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
        {
            Debug.LogError("MonsterSpawner: No monster prefabs assigned!");
        }
    }

    public void SpawnInitialMonsters()
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
        {
            Debug.LogError("Cannot spawn monsters: No prefabs assigned!");
            return;
        }

        if (enableDebugLogs) Debug.Log($"Spawning {monsterCount} monsters at {transform.position}");

        for (int i = 0; i < monsterCount; i++)
        {
            // Create position in a circle around the spawner
            float angle = (i / (float)monsterCount) * 360f * Mathf.Deg2Rad;
            float distance = Random.Range(spawnRadius * 0.5f, spawnRadius); // More consistent spread using larger range
            
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * distance,
                Random.Range(minHeight, maxHeight),
                Mathf.Sin(angle) * distance
            );
            
            Vector3 spawnPos = transform.position + offset;
            
            GameObject prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
            
            if (prefab == null)
            {
                Debug.LogError($"Monster prefab at index is null! Skipping spawn {i}");
                continue;
            }

            // Instantiate as child of spawner to maintain AR tracking
            GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
            
            // Make monster face the center
            monster.transform.LookAt(transform.position);
            monster.transform.Rotate(0, 180, 0); // Flip to face outward
            
            // Ensure monster has correct tag
            if (!monster.CompareTag("Monster"))
            {
                monster.tag = "Monster";
                if (enableDebugLogs) Debug.Log($"Added 'Monster' tag to {monster.name}");
            }
            
            // Ensure monster has collider for tap detection
            BoxCollider collider = monster.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = monster.AddComponent<BoxCollider>();
                if (enableDebugLogs) Debug.Log($"Added BoxCollider to {monster.name}");
            }
            
            // IMPORTANT: Ensure Is Trigger is FALSE for raycast detection
            collider.isTrigger = false;
            
            // Optional: Adjust collider size to fit monster bounds better
            Renderer renderer = monster.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                collider.center = renderer.bounds.center - monster.transform.position;
                collider.size = renderer.bounds.size;
                if (enableDebugLogs) Debug.Log($"Adjusted collider to match renderer bounds");
            }
            
            if (enableDebugLogs) 
            {
                Debug.Log($"Spawned monster {i + 1}: {monster.name} at {spawnPos}");
            }
        }
        
        if (enableDebugLogs) Debug.Log("Monster spawning complete.");
    }
}
