using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;
    public int monsterCount = 3;

    public void SpawnInitialMonsters()
    {
        for (int i = 0; i < monsterCount; i++)
        {
            Vector3 spawnPos = transform.position +
                               new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(0.3f, 1.0f));
            Instantiate(monsterPrefabs[Random.Range(0, monsterPrefabs.Length)],
                        spawnPos, Quaternion.identity, transform);
        }
    }
}
