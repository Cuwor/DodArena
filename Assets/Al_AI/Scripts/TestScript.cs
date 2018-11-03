using UnityEngine;
using UnityEngine.AI;

namespace Al_AI.Scripts
{
	public class TestScript : MyTools
	{
		public NavMeshAgent NavAgent;
		public Transform goal;
		public Animator anim;
		public bool key;

		// Use this for initialization
		void Start ()
		{
			NavAgent = gameObject.GetComponent<NavMeshAgent>();
			anim = GetComponent<Animator>();
		
		}
	
		// Update is called once per frame
		void Update ()
		{
			NavAgent.SetDestination(goal.position);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (key)
			{
				ObstacleScript OS;
				if (MyGetComponent(out OS, other.gameObject))
				{
					

					anim.SetTrigger("Spec");
					key = false;
				}

				
			}
			else
			{
				key = true;
			}
			
		}
	}
	
}
