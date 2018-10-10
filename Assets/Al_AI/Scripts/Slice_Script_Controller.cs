using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Slice_Script_Controller : MyTools, IAlive {
	[Tooltip("Здесь объект")]
	[Header("Здесь объект")]
	public GameObject Player;
	[Space(20)]
	public NavMeshAgent NavAgent;
	public float DistanceTP;
	[Range(1,10)]
	public float RadiusAttack;
	[Range(0,100)]
	public float HP;
    public bool saled;
    public bool key;



    private Animator _anim;
	private int qtyEidolons = 4;
    private float maxHP;

	public float Health
	{
		get
		{
			return HP;
		}

		set
		{
			if(value <= 0)
			{
				Death();

				HP = 0;
			}
			HP = value;
		}
	}
	
	public GameObject Eidolon;

	// Use this for initialization
	void Start ()
	{
		NavAgent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
        key = saled = true;
        maxHP = Health;
	}
	
	// Update is called once per frame
	void Update ()
	{
		DistanceTP = Vector3.Distance(Player.transform.position, transform.position);
		if (DistanceTP > RadiusAttack)
		{
			NavAgent.enabled = true;
			NavAgent.destination = Player.transform.position;
		}
		else if (DistanceTP <= RadiusAttack)
		{
			NavAgent.enabled = false;
		}

	}

	IEnumerator CreateEidolons()
	{
		yield return new WaitForSeconds(1f);
		GameObject eidolon;
		for (int i = 0; i < qtyEidolons; i++)
		{
			eidolon = Instantiate(Eidolon, GetRandomPositionForEidolons(), Quaternion.identity);
            Slice_Script_Controller SSC;
            if(MyGetComponent(out SSC, eidolon))
            {
                SSC.Player = Player;
                SSC.Health = maxHP / 2;
            }
        }
	}
	IEnumerator Destroeded()
	{
		yield return new WaitForSeconds(2);
		Destroy(gameObject);
	}

	
	public void GetDamage(float value)
	{
		Health -= value;
		_anim.SetInteger("Damage", (int)value);
		_anim.SetTrigger("GetDamage");
		Debug.Log(Health);
	}

	public void PlusHealth(float value)
	{
		
	}

	public void Death()
	{
		_anim.SetTrigger("Dead");
		if(saled)
			StartCoroutine(CreateEidolons());
		StartCoroutine(Destroeded());
		key = false;
	}

    private Vector3 GetRandomPositionForEidolons()
    {
        float x, z;
        x = UnityEngine.Random.Range(-2, 3);
        z = UnityEngine.Random.Range(-2, 3);

        return transform.position + new Vector3(x, 0, z);
    }

    private void OnTriggerEnter(Collider other)
	{
		Projectile proj;
		if(MyGetComponent(out proj, other.gameObject))
		{
			GetDamage(proj.damage);
		}
	}
}

