using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerWave : MonoBehaviour
{
    public List<GameObject> EnemysToSpawn;
    public WaveManager WaveManager;
    public bool key = false;
    private int i = 0;

    private void Awake()
    {
        WaveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
    }

    private void Update()
    {
        if (key && i < EnemysToSpawn.Count)
        {
            SpawnEnemy(EnemysToSpawn[i], this.transform.position);
            i++;
            
        }
    }
    public void SpawnEnemy(GameObject enemy, Vector3 pos)
    {
        if (enemy != null)
        {
            var temp = Instantiate(enemy, pos, Quaternion.identity);
            WaveManager.AddEnemy(temp);
        }
        key = false;
    }
}