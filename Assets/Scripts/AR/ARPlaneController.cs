using System;
using UnityEngine;
using Vuforia;

public class ARPlaneController : MonoBehaviour
{
    public MonsterSpawner spawner;
    private bool planeDetected = false;

    void Start()
    {
        VuforiaBehaviour.Instance.World.OnObserverCreated += OnPlaneDetected;
    }

    void OnPlaneDetected(ObserverBehaviour behaviour)
    {
        if (planeDetected) return;

        if (behaviour == null)
            return;

        var typeName = behaviour.GetType().Name;
        
        if (typeName.IndexOf("Plane", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            planeDetected = true;
            Debug.Log("Ground Plane detected. Spawning monsters...");
            spawner.SpawnInitialMonsters();
        }
    }
}
