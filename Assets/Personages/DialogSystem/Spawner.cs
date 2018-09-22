using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : UsedObject {

    public GameObject[] spawnObjects;

    public override void Use()
    {
        foreach(var c in spawnObjects)
        {
            Instantiate(c, GetRandomPosition(), Quaternion.identity);
        }
        
    }

    private Vector3 GetRandomPosition()
    {
        float x, z;
        x = Random.Range(- 5, 6);
        z = Random.Range(- 5, 6);

        return transform.position + new Vector3(x, 0, z);
    }
}
