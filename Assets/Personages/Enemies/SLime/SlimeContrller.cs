using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeContrller : MyTools, IAlive {

    public GameObject player;
    public GameObject deadBody;


    private NavMeshAgent agent;
    private Rigidbody rb;
    private float health;
    private Vector3 impulseVector;




    public float Health
    {
        get
        {
            return health;
        }

        set
        {
            if(value < 0)
            {
                Death();

                health = 0;
            }
            health = value;
        }
    }

    public void Death()
    {
        throw new System.NotImplementedException();
    }

    public void GetDamage(float value)
    {
        rb.AddForce(impulseVector * value);
        Health -= value;
    }

    public void PlusHealth(float value)
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(player.transform.position);
        Wait();
	}

    private Vector3 GetPlayerPosition()
    {
        return new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        agent.destination = GetPlayerPosition();
    }


    private void OnTriggerEnter(Collider other)
    {
        Projectile proj;
        if(MyGetComponent(out proj, other.gameObject))
        {
            impulseVector = transform.position - other.transform.position;
            GetDamage(proj.damage);
        }
    }
}
