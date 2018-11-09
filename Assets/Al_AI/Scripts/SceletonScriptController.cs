using System.Collections;
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

			NavAgent = nav;
			Initiolize();
			alive = false;
			
		}

		public override void Initiolize()
		{
			radio = GameObject.FindGameObjectWithTag("Radio");
			WS = SceneManager.GetSceneByBuildIndex(3);
			TS = SceneManager.GetSceneByBuildIndex(4);
			FindPlayers();
			gameObject.AddComponent<Rigidbody>().isKinematic = true;
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

		protected override IEnumerator Destroeded()
		{
            PlayThisClip(audioSource, Moves[2]);
            yield return new WaitForSeconds(2);
			Destroy(transform.parent.gameObject);
		}
	
		public override void GetDamage(float value)
		{
            if(Health > 0)
            {
	            Alarm = true;
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
				if (alive)
				{
                
					DistanceTP = Vector3.Distance(target.transform.position, transform.position);
               
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
