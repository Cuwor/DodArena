using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float damage;
    private BoxCollider box;

    private void Start()
    {
        Invoke("DestroyProjectile", 3f);
        box = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (damage - 0.0005f * Time.deltaTime > 0)
        {
            damage -= 0.0005f * Time.deltaTime;
            box.size *= 1.005f;
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
