using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HitHelper();

public class DamageScript : MonoBehaviour
{
    private float damage;
    private float pauseTime;
    private short bulletInShoot;
    private bool auto;
    private float radius;

    public float PauseTime
    {
        get { return pauseTime; }

        set
        {
            pauseTime = value;
            burst.repeatInterval = value;
            //main.duration = value;
            emission.SetBurst(0, burst);
        }
    }

    public short BulletInShoot
    {
        get { return bulletInShoot; }
        set
        {
            bulletInShoot = value;
            burst.count = value;
            emission.SetBurst(0, burst);
        }
    }

    public float Damage
    {
        get
        {
            HitInvoke();
            return damage;
        }

        set { damage = value; }
    }

    public float Radius
    {
        get { return radius; }

        set
        {
            radius = value;
            shape.angle = value;
        }
    }

    public bool Auto
    {
        get { return auto; }

        set { auto = value; }
    }

    private ParticleSystem.Burst burst;
    private ParticleSystem.Burst burst1;
    private new ParticleSystem particleSystem;
    private ParticleSystem.ShapeModule shape;
    //private ParticleSystem.MainModule main;
    private ParticleSystem.EmissionModule emission;
    public event HitHelper hitHandler;

    public GameObject cam;

    // Use this for initialization
    private void Awake()
    {
        particleSystem = gameObject.GetComponent<ParticleSystem>();
        shape = particleSystem.shape;
        //main = particleSystem.main;
        emission = particleSystem.emission;
        burst = new ParticleSystem.Burst(0, BulletInShoot, 1, PauseTime);
        emission.SetBurst(0, burst);
    }


    public void Fire(bool start)
    {
        if (start)
        {
            RaycastHit hit;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            var mask = (1 << 10);
            if (Physics.Raycast(ray, out hit, 1000, mask))
            {
                transform.LookAt(hit.point);
            }
            else
            {
                transform.localRotation = new Quaternion();
            }
            particleSystem.Play();
        }
        else
            particleSystem.Stop();
    }

    public void HitInvoke()
    {
        if (hitHandler != null)
            hitHandler.Invoke();
    }
}
