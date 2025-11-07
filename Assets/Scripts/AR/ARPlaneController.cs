using System;
using UnityEngine;
using Vuforia;

public class ARPlaneController : MonoBehaviour
{
    public MonsterSpawner spawner;
    public bool enableDebugLogs = true;
    public bool spawnOnFirstDetection = true; // Spawn automatically on first plane detection
    public bool requireTapToPlace = false; // If true, user must tap to place monsters
    public GameObject[] objectsToHideAfterSpawn; // Array of objects to hide when monsters spawn
    public float respawnCheckInterval = 2f; // How often to check if monsters need respawning
    private bool planeDetected = false;
    private bool monstersSpawned = false;
    private PlaneFinderBehaviour planeFinder;
    private ContentPositioningBehaviour contentPositioning;
    private float nextRespawnCheck = 0f;

    void Start()
    {
        if (spawner == null)
        {
            Debug.LogError("MonsterSpawner not assigned to ARPlaneController!");
            return;
        }

        // Find PlaneFinderBehaviour in the scene
        planeFinder = FindObjectOfType<PlaneFinderBehaviour>();
        if (planeFinder != null)
        {
            if (enableDebugLogs) Debug.Log("PlaneFinderBehaviour found.");
            
            // Subscribe to hit test events
            planeFinder.OnInteractiveHitTest.AddListener(OnInteractiveHitTest);
            planeFinder.OnAutomaticHitTest.AddListener(OnAutomaticHitTest);
        }
        else
        {
            Debug.LogError("PlaneFinderBehaviour not found in scene! Please add a Plane Finder to your scene.");
            return;
        }

        // Find or add ContentPositioningBehaviour
        contentPositioning = GetComponent<ContentPositioningBehaviour>();
        if (contentPositioning == null)
        {
            contentPositioning = gameObject.AddComponent<ContentPositioningBehaviour>();
            if (enableDebugLogs) Debug.Log("Added ContentPositioningBehaviour component.");
        }

        // Configure content positioning
        contentPositioning.AnchorStage = planeFinder.GetComponent<AnchorBehaviour>();
        contentPositioning.enabled = true;

        if (enableDebugLogs) Debug.Log("ARPlaneController initialized. Waiting for plane detection...");
    }

    void OnDestroy()
    {
        if (planeFinder != null)
        {
            planeFinder.OnInteractiveHitTest.RemoveListener(OnInteractiveHitTest);
            planeFinder.OnAutomaticHitTest.RemoveListener(OnAutomaticHitTest);
        }
    }

    void Update()
    {
        // Check if we need to respawn monsters
        if (planeDetected && monstersSpawned && Time.time >= nextRespawnCheck)
        {
            nextRespawnCheck = Time.time + respawnCheckInterval;
            CheckAndRespawnMonsters();
        }
    }

    private void OnInteractiveHitTest(HitTestResult result)
    {
        if (enableDebugLogs) Debug.Log($"Interactive hit test at position: {result.Position}");
        
        // Interactive hit test means user tapped the screen
        HandleHitTest(result, true);
    }

    private void OnAutomaticHitTest(HitTestResult result)
    {
        if (enableDebugLogs) Debug.Log($"Automatic hit test at position: {result.Position}");
        
        // Automatic hit test means Vuforia found a plane automatically
        planeDetected = true;
        
        // Only spawn on automatic detection if configured to do so
        if (spawnOnFirstDetection && !requireTapToPlace)
        {
            HandleHitTest(result, false);
        }
    }

    private void HandleHitTest(HitTestResult result, bool isInteractive)
    {
        if (monstersSpawned)
        {
            if (enableDebugLogs) Debug.Log("Monsters already spawned, ignoring hit test.");
            return;
        }

        if (spawner == null)
        {
            Debug.LogError("Cannot spawn monsters: MonsterSpawner is null!");
            return;
        }

        // Position the spawner at the hit location
        spawner.transform.position = result.Position;
        spawner.transform.rotation = result.Rotation;

        // Spawn monsters
        monstersSpawned = true;
        string detectionType = isInteractive ? "interactive tap" : "automatic detection";
        Debug.Log($"Plane detected via {detectionType}. Spawning monsters at {result.Position}...");
        
        spawner.SpawnInitialMonsters();
        
        Debug.Log($"Spawned {spawner.monsterCount} monsters successfully.");

        // Hide specified objects after spawning
        HideObjects();

        // Optionally disable plane finder visualization after spawning
        if (planeFinder != null)
        {
            var planeRenderer = planeFinder.GetComponent<Renderer>();
            if (planeRenderer != null)
            {
                planeRenderer.enabled = false; // Hide the plane indicator
            }
        }
    }

    // Public method to manually trigger spawn (useful for testing or UI button)
    public void ManualSpawn()
    {
        if (monstersSpawned)
        {
            Debug.LogWarning("Monsters already spawned!");
            return;
        }

        if (!planeDetected)
        {
            Debug.LogWarning("No plane detected yet. Please scan the ground first.");
            return;
        }

        // Use spawner's current position
        Debug.Log("Manual spawn triggered!");
        monstersSpawned = true;
        spawner.SpawnInitialMonsters();
        
        // Hide specified objects after spawning
        HideObjects();
    }

    private void HideObjects()
    {
        if (objectsToHideAfterSpawn != null && objectsToHideAfterSpawn.Length > 0)
        {
            foreach (GameObject obj in objectsToHideAfterSpawn)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    if (enableDebugLogs) Debug.Log($"Hidden object: {obj.name}");
                }
            }
        }
    }

    private void CheckAndRespawnMonsters()
    {
        // Check if any monsters still exist
        GameObject[] existingMonsters = GameObject.FindGameObjectsWithTag("Monster");
        
        if (existingMonsters.Length == 0)
        {
            if (enableDebugLogs) Debug.Log("No monsters found. Respawning monsters...");
            
            // Respawn monsters at the spawner's current position
            if (spawner != null)
            {
                spawner.SpawnInitialMonsters();
                Debug.Log($"Respawned {spawner.monsterCount} monsters successfully.");
            }
            else
            {
                Debug.LogError("Cannot respawn: MonsterSpawner is null!");
            }
        }
    }
}
