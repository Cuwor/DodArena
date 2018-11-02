using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceletonScriptController : Monster
{
	public GameObject[] sceletonparts;
	public bool key1 = false;
	public bool key2 = false;
	public bool key3 = false;
	public int phase = 1;

	// Use this for initialization
	void Start () {
		Initiolize();
	}

	public override float Health
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
			else if (value <= 75 && value > 50 && !key1)
			{
				key1 = true;
				phase++;
				sceletonparts[0].GetComponent<Rigidbody>().useGravity = true;
				sceletonparts[0].transform.SetParent(null);
			}
			else if (value <= 50 && value > 25 && !key2)
			{
				phase++;
				key2 = true;
				sceletonparts[1].GetComponent<Rigidbody>().useGravity = true;
				sceletonparts[1].transform.SetParent(null);
			}
			else if (value <= 25 && value > 0 && !key3)
			{
				phase++;
				key3 = true;
				sceletonparts[2].GetComponent<Rigidbody>().useGravity = true;
				sceletonparts[2].transform.SetParent(null);
			}

			HP = value;
		}
		
	}
	
	public override void GetDamage(float value)
	{
		Health -= value;
		_anim.SetInteger("Health", (int)Health);
		_anim.SetTrigger("GetDamage");
	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (alive)
		{
			if (target != null)
			{
				distanceTP = Vector3.Distance(target.transform.position, transform.position);
				if (distanceTP <= RadiusView && distanceTP > attackDistance)
				{
					State = EnemyState.Walk;
					
					
				}
				else if (distanceTP <= attackDistance)
				{
					State = EnemyState.Attack;

				}
				else
				{
					State = EnemyState.Stay;
				}
			}
			else
			{
				FindPlayers();
			}
			
			switch (State)
			{
				case EnemyState.Stay:
					if (!wait)
					{
						wait = true;  CaseMethod(false, UnityEngine.Random.Range(-1, 1.1f), -1, 0, target.transform.position);
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

				
			}
		}
		
	}

	
}
