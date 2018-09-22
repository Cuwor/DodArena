using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float damage;

    private void Update()
    {
        if(damage - 0.05f > 0)
        {
            damage -= 3f;
        }
    }

    private void Start()
    {
        Invoke("DestroyProjectile", 3f);
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }

}
