using UnityEngine;
using UnityEngine.AI;

namespace Al_AI.Scripts
{
	public class TestScript : MyTools
	{
		public NavMeshAgent NavAgent;
		public Transform goal;

		// Use this for initialization
		void Start ()
		{
			NavAgent = gameObject.GetComponent<NavMeshAgent>();
		}
	
		// Update is called once per frame
		void Update ()
		{
			NavAgent.SetDestination(goal.position);
		}

	}
}
