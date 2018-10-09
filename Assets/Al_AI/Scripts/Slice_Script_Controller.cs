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
	[Range(90,100)]
	public float HP =95;

	public bool saled = true;

	private int qtyEidolons = 4;
	private Animator _anim;
	public bool key = true;

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
	
	public GameObject[] Eidolons;

	// Use this for initialization
	void Start ()
	{
		NavAgent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (HP <= 0 && key)
		{
			Death();
		}
		
		DistanceTP = Vector3.Distance(Player.transform.position, transform.position);
//		Debug.Log(DistanceTP);
		if (DistanceTP > RadiusAttack)
		{
			NavAgent.enabled = true;
			NavAgent.destination = Player.transform.position;
		}
		else if (DistanceTP<=RadiusAttack)
		{
			NavAgent.enabled = false;
			//HP -= 100;
		}

	}

	IEnumerator CreateEidolons()
	{
		yield return new WaitForSeconds(1f);
		GameObject eidolon;
		for (int i = 0; i < qtyEidolons; i++)
		{
			eidolon = Instantiate(Eidolons[0], transform.position, Quaternion.identity);
			eidolon.GetComponent<Slice_Script_Controller>().Player = Player;
		}
	}
	IEnumerator Destroeded()
	{
		yield return new WaitForSeconds(2);
		Destroy(this.gameObject);
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
	
	private void OnTriggerEnter(Collider other)
	{
		Projectile proj;
		if(MyGetComponent(out proj, other.gameObject))
		{
			GetDamage(proj.damage);
		}
	}
}

