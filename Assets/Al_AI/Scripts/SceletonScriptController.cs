using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Al_AI.Scripts
{
	public class SceletonScriptController : Monster
	{
		public NavMeshAgent nav;
		public GameObject[] sceletonparts;
		public bool key1 = false;
		public bool key2 = false;
		public bool key3 = false;
		public int phase = 1;
		

		// Use this for initialization
		void Start () 
		{
			Initiolize();
			NavAgent = nav;
            alive = false;
			
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
					phase = 2;
					sceletonparts[0].GetComponent<Rigidbody>().useGravity = true;
					sceletonparts[0].transform.SetParent(null);
				}
				else if (value <= 50 && value > 25 && !key2)
				{
					phase = 3;
					key2 = true;
					sceletonparts[1].GetComponent<Rigidbody>().useGravity = true;
					sceletonparts[1].transform.SetParent(null);
				}
				else if (value <= 25 && value > 0 && !key3)
				{
					phase = 4;
					key3 = true;
					sceletonparts[2].GetComponent<Rigidbody>().useGravity = true;
					sceletonparts[2].transform.SetParent(null);
				}
                _anim.SetInteger("phase", phase);
                HP = value;
			}
		
		}
	
		public override void GetDamage(float value)
		{
            if(Health > 0)
            {
                alive = false;
                NavAgent.enabled = false;
                Health -= value;
                _anim.SetInteger("Health", (int)Health);
                _anim.SetTrigger("GetDamage");
            }
		}
		// Update is called once per frame
		void FixedUpdate () 
		{
			
			if (alive)
			{
				if (SceneManager.GetActiveScene().name == WS.name && true) // WaveMode is on && radio is on
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
							NavAgent.enabled = false;
						}
						else // движение и атака радио игорь помоги портировать с демонконтроллера систему аларма
						{
							if (Alarm)
							{
								//???
							}
							else // хуйня какая то
							{
								target = radio;
							}
						}
					}
					else
					{
						FindPlayers();
					}
				}
				else if (SceneManager.GetActiveScene().name == TS.name && true) // TimerMode is on && radio is on
				{
					if (target != null)
					{
						distanceTP = Vector3.Distance(target.transform.position, transform.position);
						if (distanceTP > attackDistance) // либо идет либо атакует
						{
							State = EnemyState.Walk;
						}
						else if (distanceTP <= attackDistance) 
						{
							State = EnemyState.Attack;
							NavAgent.enabled = false;
						}
					}
					else
					{
						FindPlayers();
					}
				}
				else //radio is off
				{
					if (target != null) // стандартный код
					{
						distanceTP = Vector3.Distance(target.transform.position, transform.position);
						if (distanceTP <= RadiusView && distanceTP > attackDistance)
						{
							State = EnemyState.Walk;
						}
						else if (distanceTP <= attackDistance)
						{
							State = EnemyState.Attack;
							NavAgent.enabled = false;
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
				}

				switch (State)
				{
					case EnemyState.Stay:
						if (!wait)
						{
							wait = true;  
							CaseMethod(false, UnityEngine.Random.Range(-1, 1.1f), -1, 0, target.transform.position);
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

        protected override void GetAttackDistance()
        {
            if(phase != 4)
            {
                attackType = UnityEngine.Random.Range(1, 3);
            }
            else
            {
                attackType = 1;
            }

            switch (phase)
            {
                case 1:
                    attackDistance = attackType == 1 ? RadiusAttack : RadiusAttack - 2;
                    break;
                case 2:
                    if(attackType == 1)
                    {
                        attackDistance = RadiusAttack - 2;
                    }
                    else
                    {
                        attackDistance = RadiusAttack / 2.1f;
                    }
                    break;
                case 3:
                    if (attackType == 1)
                    {
                    attackDistance = RadiusAttack / 2;
                    }
                    else
                    {
                        attackDistance = RadiusAttack / 2.1f;
                    }
                    break;
                case 4:
                    attackDistance = RadiusAttack / 2.1f;
                    break;
            }
        }
    }
}
