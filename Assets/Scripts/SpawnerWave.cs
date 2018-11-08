using System.Collections;
using System.Collections.Generic;
using Al_AI.Scripts;
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
        if (key)
        {
            SpawnEnemy(EnemysToSpawn[i], this.transform.position);
            i++;
            if (i == EnemysToSpawn.Count)
            {
                i = 0;
            }
            
        }
    }
    public void SpawnEnemy(GameObject enemy, Vector3 pos)
    {
        if (enemy != null)
        {
            var temp = Instantiate(enemy, pos, Quaternion.identity);
            WaveManager.AddEnemy(temp);
            temp.GetComponent<Monster>().radio = WaveManager.radio.gameObject;
        }
        key = false;
    }
}