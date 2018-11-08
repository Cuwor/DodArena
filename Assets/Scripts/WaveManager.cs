using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public bool SpawnKey = false;
    public List<GameObject> Spawners;
    public List<GameObject> enemies;

    private void Update()
    {
        if (SpawnKey)
        {
            foreach (var spawner in Spawners)
            {
                spawner.GetComponent<SpawnerWave>().key = true;
            }

            SpawnKey = false;
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
}