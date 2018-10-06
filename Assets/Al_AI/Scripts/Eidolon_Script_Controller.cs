using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Eidolon_Script_Controller : MonoBehaviour {

	public GameObject Player;
	[Space(20)]
	public NavMeshAgent NavAgent;
	public float DistanceTP;
	[Range(1,10)]
	public float RadiusAttack=2;
	[Range(90,100)]
	public float HP =95;
	// Use this for initialization
	void Start () {
		NavAgent = GetComponent<NavMeshAgent>();
		Player = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
		if (HP <= 0)
		{
			StartCoroutine(Destroeded());
		}
		DistanceTP = Vector3.Distance(Player.transform.position, transform.position);
		Debug.Log(DistanceTP);
		if (DistanceTP > RadiusAttack)
		{
			NavAgent.enabled = true;
			NavAgent.destination = Player.transform.position;
		}
		else if (DistanceTP<=RadiusAttack)
		{
			NavAgent.enabled = false;
			HP -= 100;
		}
		
	}

	IEnumerator Destroeded()
	{
		yield return new WaitForSeconds(2);
		Destroy(this.gameObject);
	}
}
