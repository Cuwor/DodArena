using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
