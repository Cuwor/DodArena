using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Al_AI.Scripts
{
	public class PumpcinheadScriptController : Monster
	{
		public ScarecrowScriptController SCSC;
		public bool OnScareCrow = true;


		void Start()
		{
			Initiolize();
			if (OnScareCrow)
			{
				NavAgent.enabled = false;
			}
			else
			{
				_anim.SetTrigger("Go");
				transform.rotation = new Quaternion(0,0,0,0);
				transform.Rotate(-90,0,0);
			}
		}
		public override void Initiolize()
		{
			radio = GameObject.FindGameObjectWithTag("Radio");
			WS = SceneManager.GetSceneByBuildIndex(3);
			TS = SceneManager.GetSceneByBuildIndex(4);
			FindPlayers();
			gameObject.AddComponent<Rigidbody>().isKinematic = true;
			NavAgent = transform.parent.gameObject.GetComponent<NavMeshAgent>();
			_anim = GetComponent<Animator>();
			maxHP = Health;
			alive = true;
			wait = false;
			for (int i = 0; i < AttackAreas.Length; i++)
			{
				AttackArea proj;
				if (MyGetComponent(out proj, AttackAreas[i]))
				{
					proj.Damage = AttackForce;
				}
			}
			StopAttack();
			State = EnemyState.Stay;
			GetAttackDistance();
			size = transform.localScale.x;
		}

		public override void Death()
		{
			NavAgent.enabled = false;
			if (SCSC != null)
			{
				SCSC.IsNotPumkinHead = true;
                SCSC.Death();
				SCSC.NavAgent.enabled = false;
			}

			_anim.SetTrigger("Dead");
			StartCoroutine(Drop());
			StartCoroutine(Destroeded());
		}

		void FixedUpdate()
		{
			if (alive && !OnScareCrow)
			{
				DistanceTP = Vector3.Distance(target.transform.position, transform.position);
			}
		}

		protected override IEnumerator Destroeded()
		{
			yield return new WaitForSeconds(2);
			Destroy(transform.parent.gameObject);
		}
		}
	}
