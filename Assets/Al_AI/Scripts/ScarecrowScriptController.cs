using System.Collections;
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
                Transform point = PHSC.transform.parent;
                point.transform.parent = null;
                point.transform.position = new Vector3(point.transform.position.x, point.transform.position.y - 1.8f, point.transform.position.z);
                PHSC._anim.enabled = true;
                PHSC._anim.SetTrigger("Go");
                PHSC.transform.localPosition = Vector3.zero;
                PHSC.transform.localRotation = new Quaternion(0, 0,0, 0);
                PHSC.transform.Rotate(-90, 0, 0);
                PHSC.SCSC = null;
                PHSC.NavAgent.enabled = true;
                PHSC.OnScareCrow = false;
            }
            _anim.SetTrigger("Dead");
			StartCoroutine(Drop());
			StartCoroutine(Destroeded());
		}



		void Start () 
		{
            Initiolize();
            
            NavAgent.enabled = false;
            Invoke("SetPumkin", 1f);
        }
		public override void Initiolize()
		{
			radio = GameObject.FindGameObjectWithTag("Radio");
			WS = SceneManager.GetSceneByBuildIndex(3);
			TS = SceneManager.GetSceneByBuildIndex(4);
			FindPlayers();
			gameObject.AddComponent<Rigidbody>().isKinematic = true;
			NavAgent = transform.parent.GetComponent<NavMeshAgent>();
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
		
        public void SetPumkin()
        {
            PHSC.gameObject.GetComponent<Rigidbody>().useGravity = false;
            PHSC.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            PHSC.NavAgent.enabled = false;
            PHSC._anim.enabled = false;
            PHSC.transform.localPosition = Vector3.zero;
        }
	
		void FixedUpdate () 
		{
			if (alive && !IsNotPumkinHead)
			{
                DistanceTP = Vector3.Distance(target.transform.position, transform.position);
			}
		
		}

        protected override void GetAttackDistance()
        {
            attackType = UnityEngine.Random.Range(1, 3);
            attackDistance = attackType == 1 ? RadiusAttack + 2 : RadiusAttack;
        }
		
		protected override IEnumerator Destroeded()
		{
			yield return new WaitForSeconds(2);
			Destroy(transform.parent.gameObject);
		}
    }
}
