using UnityEngine;
using UnityEngine.AI;

namespace Al_AI.Scripts.Scripts_from_other_projects
{
	public class Move_Script : MonoBehaviour {

		public Transform goal;
		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update ()
		{
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			agent.destination = goal.position; 
		}
	}
}
