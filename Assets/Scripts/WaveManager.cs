using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void OnWaveHandler(int number);

public class WaveManager : MonoBehaviour
{
    public List<GameObject> Spawners;
    public List<GameObject> enemies;
    public Dialog radio;
    private bool keyForFunction;
    public PlayerUI PUI;
    public event OnWaveHandler NewWave;
    
    private int savedWave;

    private void Update()
    {
        if (keyForFunction)
        {
            StartCoroutine(Waiterforseconds());

        }
    }

    public void FunctionForKey()
    {
        keyForFunction = true;
    }

    private void Start()
    {
        
        NewWave += PUI.DrawWave;
        keyForFunction = false;
        Dialog.OnStart += FunctionForKey;
        savedWave = 0;
    }

    public void Spawn()
    {
        foreach (var spawner in Spawners)
        {
            spawner.GetComponent<SpawnerWave>().key = true;
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    private IEnumerator Waiterforseconds()
    {
        keyForFunction = false;
        yield return new WaitForSeconds(5f);
        keyForFunction = true;
        bool key = true;
        foreach (var n in enemies)
        {
            if (n != null)
            {
                key = false;
                break;
            }
        }

        if (key)
        {
            enemies = new List<GameObject>();
            savedWave++;
            if (NewWave != null)
            {
                NewWave.Invoke(savedWave);
            }
            Spawn();
        }
    }
}