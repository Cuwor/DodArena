using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PumpcinheadScriptController : Monster
{
	public ScarecrowScriptController SCSC;
	public bool OnScareCrow;
	
	
	void Start () 
	{
		Initiolize();
	}
	
	void FixedUpdate () 
	{
		if (alive && !OnScareCrow)
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
