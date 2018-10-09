using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float damage;

    private void Update()
    {
//        if(damage - 0.0005f*Time.deltaTime > 0)
//        {
//            damage -= 0.0005f*Time.deltaTime;
//        }
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
