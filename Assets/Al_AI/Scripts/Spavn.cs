using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spavn : MonoBehaviour
{
    public float min = -2;
    public float max = 3;
    public float cooldown = 5;
    public GameObject enemy;

    // Use this for initialization
    private void Start()
    {
        State();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private Vector3 GetRandomPositionForEidolons()
    {
        float x, z;
        x = UnityEngine.Random.Range(min, max);
        z = UnityEngine.Random.Range(min, max);

        return transform.position + new Vector3(x, 0, z);
    }

    private void State()
    {
        Instantiate(enemy, GetRandomPositionForEidolons(), Quaternion.identity);
        Invoke("State", cooldown);
    }
}
