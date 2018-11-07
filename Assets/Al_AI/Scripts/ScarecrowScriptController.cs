using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace Al_AI.Scripts
{
	public class ScarecrowScriptController : Monster
	{
        public PumpcinheadScriptController PHSC;
		public bool IsNotPumkinHead = false;

		public override void Death()
		{
		
			NavAgent.enabled = false;
			_anim.enabled = true;
			if (!IsNotPumkinHead)
			{
				PHSC._anim.enabled = true;
				PHSC.OnScareCrow = false;
				PHSC.gameObject.transform.parent = null;
			}

			_anim.SetTrigger("Dead");
			StartCoroutine(Drop());
			StartCoroutine(Destroeded());
		}



		void Start () 
		{
			Initiolize();
            NavAgent = transform.parent.GetComponent<NavMeshAgent>();
			PHSC._anim.enabled = false;
			PHSC.NavAgent.enabled = false;
		}
	
		void FixedUpdate () 
		{
			if (alive && !IsNotPumkinHead)
			{
                DistanceTP = Vector3.Distance(target.transform.position, transform.position);
			
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

        protected override void GetAttackDistance()
        {
            attackType = UnityEngine.Random.Range(1, 3);
            attackDistance = attackType == 1 ? RadiusAttack + 2 : RadiusAttack;
        }
    }
}
