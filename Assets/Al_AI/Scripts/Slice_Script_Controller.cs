using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public enum EnemyState
{
	Stay,
	Attack,
	Walk,
	Spec,
}

public enum SlimeState
{
	Parts,
	Alone
}

public class Slice_Script_Controller : MyTools, IAlive
{
	[Tooltip("Здесь объект")] [Header("Здесь объект")]
	public GameObject target;

	public GameObject unionTarget;
	[Space(20)] public NavMeshAgent NavAgent;
	public float DistanceTP;
	public float distanceUT;
	[Range(1, 30)] public float RadiusAttack;
	[Range(1, 100)] public float RadiusView;
	[Range(0, 100)] public float HP;
	[Range(0, 50)] public float AttackForce;
	public GameObject Eidolon;
	public GameObject god;
	public bool saled;
	public bool boss;
	public bool itIsBig;
	public bool ready;
	public int bossReady;


	public GameObject[] AttackAreas;

	private EnemyState state;
	private Animator _anim;
	private int qtyEidolons = 4;
	private int attackType;
	private float maxHP;
	private float attackDistance;
	private float size;
	private bool alive;
	private bool wait;

	//[HideInInspector]
	public List<GameObject> Brothers;

	public float Health
	{
		get { return HP; }

		set
		{
			if (value <= 0)
			{
				Death();
				alive = false;
				HP = 0;
			}

			HP = value;
		}
	}


	// Use this for initialization
	public void Start()
	{
		Initiolize();
	}

	// Update is called once per frame
	void Update()
	{
		if (alive)
		{
			if (target != null)
			{
				DistanceTP = Vector3.Distance(target.transform.position, transform.position);
				if (!boss && !itIsBig)
				{
					distanceUT = Vector3.Distance(unionTarget.transform.position, transform.position);
				}

				if (DistanceTP <= RadiusView && DistanceTP > RadiusAttack)
				{
					state = EnemyState.Walk;
				}
				else if (DistanceTP <= attackDistance)
				{
					state = EnemyState.Attack;
				}
				else if (!itIsBig && Brothers.Count > qtyEidolons-2)
				{
					if (!boss)
					{
						if (distanceUT > RadiusAttack)
						{
							state = EnemyState.Spec;
						}
						else
						{
							state = EnemyState.Stay;
							if (!ready)
							{
								ready = true;
								unionTarget.GetComponent<Slice_Script_Controller>().bossReady++;
							}
						}
					}
					else
					{
						if (bossReady > qtyEidolons-2 && !ready)
						{
							StartCoroutine(Unite());
							ready = true;
						}
					}
				}
				else
				{
					state = EnemyState.Stay;
				}
			}
			else
			{
				FindPlayers();
			}

			switch (state)
			{
				case EnemyState.Stay:
					if (!wait)
					{
						wait = true;
						StartCoroutine(GetRandomStayState());
					}

					break;

				case EnemyState.Walk:
					CaseMethod(true, 0, 1, 0, target.transform.position);
					break;

				case EnemyState.Attack:
					GetAttackDistance();
					CaseMethod(false, 0, 0, attackType, target.transform.position);
					break;
				case EnemyState.Spec:
					CaseMethod(true, 0, 1, 0, unionTarget.transform.position);
					break;
			}
		}
	}

	IEnumerator Unite()
	{			
		foreach (var VARIABLE in Brothers)
		{
			var c = VARIABLE.GetComponent<Slice_Script_Controller>();
			c.saled = true;
			c.Death();
		}
		NavAgent.enabled = false;
		_anim.SetTrigger("Dead");
		StartCoroutine(Destroeded());
		
		yield return new WaitForSeconds(1f);
		Instantiate(god, GetRandomPositionForEidolons(), Quaternion.identity);
	}
	
	
	private void CaseMethod(bool navAgentEnebled, float xstate, float ysate, int attack, Vector3 destenation)
	{
		NavAgent.enabled = navAgentEnebled;
		NavAgent.destination = destenation;
		_anim.SetInteger("Attack", attack);
		_anim.SetFloat("Xstate", xstate);
		_anim.SetFloat("Ystate", ysate);
	}

	public void Initiolize()
	{
		FindPlayers();
		NavAgent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
		maxHP = Health;
		alive = true;
		wait = false;
		boss = false;
		ready = false;
		bossReady = 0;
		for (int i = 0; i < AttackAreas.Length; i++)
		{
			AttackArea proj;
			if (MyGetComponent(out proj, AttackAreas[i]))
			{
				proj.Damage = AttackForce;
			}
		}

		state = EnemyState.Stay;
		GetAttackDistance();
		size = transform.localScale.x;
	}


	IEnumerator CreateEidolons()
	{
		yield return new WaitForSeconds(1f);
		GameObject[] eidolons = new GameObject[qtyEidolons];
		for (int i = 0; i < qtyEidolons; i++)
		{
			eidolons[i] = Instantiate(Eidolon, GetRandomPositionForEidolons(), Quaternion.identity);
			Slice_Script_Controller SSC;
			if (MyGetComponent(out SSC, eidolons[i]))
			{
				SSC.Initiolize();
				SSC.target = target;
				SSC.Health = maxHP / 2;
				SSC.AttackForce = AttackForce / 2;
				SSC.RadiusView = RadiusView - 2;
				SSC.NavAgent.speed = NavAgent.speed / size;
			}
		}

		eidolons[0].GetComponent<Slice_Script_Controller>().boss = true;
		for (int i = 0; i < eidolons.Length; i++)
		{
			Slice_Script_Controller SSC = eidolons[i].GetComponent<Slice_Script_Controller>();
			if (i == 0)
			{
				SSC.boss = true;
			}

			SSC.Brothers = new List<GameObject>();
			SSC.unionTarget = eidolons[0];
			for (int j = 0; j < eidolons.Length; j++)
			{
				if (i != j)
				{
					SSC.Brothers.Add(eidolons[j]);
				}
			}
		}
	}

	IEnumerator Destroeded()
	{
		yield return new WaitForSeconds(2);
		Destroy(gameObject);
	}

	IEnumerator GetRandomStayState()
	{
		_anim.SetFloat("Ystate", -1);
		_anim.SetFloat("Xstate", UnityEngine.Random.Range(-1, 1.1f));
		FindPlayers();
		yield return new WaitForSeconds(2);
		wait = false;
	}

	public void GetDamage(float value)
	{
		Health -= value;
		_anim.SetInteger("Damage", (int) value);
		_anim.SetTrigger("GetDamage");
	}

	public void PlusHealth(float value)
	{
	}

	public void Death()
	{
		NavAgent.enabled = false;
		_anim.SetTrigger("Dead");
		if (saled)
			StartCoroutine(CreateEidolons());
		StartCoroutine(Destroeded());
		foreach (var VARIABLE in Brothers)
		{
			VARIABLE.GetComponent<Slice_Script_Controller>().Brothers.Remove(this.gameObject);
		}
	}

	private void GetAttackDistance()
	{
		attackType = UnityEngine.Random.Range(1, 3);
		attackDistance = attackType == 1 ? RadiusAttack : RadiusAttack - 2 * size;
	}

	private Vector3 GetRandomPositionForEidolons()
	{
		float x, z;
		x = UnityEngine.Random.Range(-2, 3);
		z = UnityEngine.Random.Range(-2, 3);

		return transform.position + new Vector3(x, 0, z);
	}

	private void FindBrothersTarget()
	{
		float distance = Vector3.Distance(transform.position, Brothers[0].transform.position);
		GameObject target = Brothers[0];

		for (int i = 1; i < Brothers.Count; i++)
		{
			float newDistance = Vector3.Distance(transform.position, Brothers[i].transform.position);
			if (newDistance < distance && newDistance > 5f)
			{
				distance = newDistance;
				target = Brothers[i];
			}
		}
	}

	private void FindPlayers()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		float distance = Vector3.Distance(transform.position, players[0].transform.position);
		target = players[0];

		for (int i = 1; i < players.Length; i++)
		{
			float newDistance = Vector3.Distance(transform.position, players[i].transform.position);
			if (newDistance < distance && newDistance > 5f)
			{
				distance = newDistance;
				target = players[i];
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (alive)
		{
			Projectile proj;
			if (MyGetComponent(out proj, other.gameObject))
			{
				GetDamage(proj.damage);
			}
		}
	}
}

