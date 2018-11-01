using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HitHelper();

public class DamageScript : MonoBehaviour
{
    private float damage;
    private float pauseTime;
    private short bulletInShoot;
    private float radius;

    public float PauseTime
    {
        get
        {
            return pauseTime;
        }

        set
        {
            pauseTime = value;
            // Debug.Log("PauseTime = " + value);
            burst1.repeatInterval = value;
            particleSystem.emission.SetBurst(1, burst1);
        }
    }

    public short BulletInShoot
    {
        get
        {
            return bulletInShoot;
        }

        set
        {
            bulletInShoot = value;
            //Debug.Log("BulletInShoot = "+ value);
            burst.cycleCount = value;
            particleSystem.emission.SetBurst(0, burst);
        }
    }

    public float Damage
    {
        get
        {
            HitInvoke();
            return damage;
        }

        set
        {
            damage = value;
        }
    }

    public float Radius
    {
        get
        {
            return radius;
        }

        set
        {
            radius = value;
        }
    }

    private ParticleSystem.Burst burst;
    private ParticleSystem.Burst burst1;
    private new ParticleSystem particleSystem;
    public event HitHelper hit;

    // Use this for initialization
    private void Start()
    {
        particleSystem = gameObject.GetComponent<ParticleSystem>();
        burst = new ParticleSystem.Burst(0, 1, BulletInShoot, 0.01f);
        burst1 = new ParticleSystem.Burst(0, 0, 1, PauseTime);
        particleSystem.emission.SetBursts(new ParticleSystem.Burst[] { burst, burst1 });
    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void Fire()
    {
        particleSystem.Play();
    }

    public void HitInvoke()
    {
        if (hit != null)
        {
            hit.Invoke();
        }
    }
}
